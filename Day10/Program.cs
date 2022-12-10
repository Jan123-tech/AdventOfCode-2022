var operations = File.ReadAllLines("data.txt")
	.Aggregate(new List<IOp>(), (ops, op) =>
	{
		var pair = op.Split(" ", StringSplitOptions.RemoveEmptyEntries);
		ops.Add(pair.First() switch
		{
			"noop" => new NoOp(),
			"addx" => new Add(int.Parse(pair.Last())),
			_ => throw new ArgumentException()
		});
		return ops;
	});

var ops = operations.SelectMany(x => Enumerable.Range(1, x.Cylces).Select(x0 => x))
	.Select((x, i) => (op: x, cycle: i + 1))
	.GroupBy(x => x.op);

var agg = ops.Aggregate(
	(reg: 1, history: new List<(int reg, IGrouping<IOp, (IOp op, int cycle)> group)>()), (agg, gOp) =>
			(gOp.Key.Operate(agg.reg), agg.history.Concat(new [] { (agg.reg, gOp) }).ToList()));

var signalStrengths = Enumerable.Range(0, 6).Select(x => x * 40 + 20)
	.Select(cycle => agg.history.First(x => x.group.Where(x0 => x0.cycle == cycle).Any()).reg * cycle);

Console.WriteLine(signalStrengths.Sum());

interface IOp
{
	int Cylces { get; }
	int Operate(int arg);
}

class NoOp : IOp
{
	public int Cylces => 1;
	public int Operate(int arg) => arg;
}

class Add : IOp
{
	readonly int value;

	public Add(int value) =>
		this.value = value;

	public int Cylces => 2;
	public int Operate(int arg) => arg + value;
}