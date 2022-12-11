// Part 1
var ops = File.ReadAllLines("data.txt")
	.Aggregate(new List<IOp>(), (ops, op) =>
		ops.Concat(new [] { CreateOp(op) }).ToList());

var agg = ops.Aggregate(
	(reg: 1, history: new List<(int reg, int cycles)>()), (agg, op) =>
		(op.Operate(agg.reg), agg.history.Concat(new [] { (agg.reg, op.Cycles) }).ToList()));

var cycles = agg.history.SelectMany(x => Enumerable.Range(1, x.cycles).Select(x0 => x.reg))
	.Select((reg, i) => (cycle: i + 1, reg));

var signalStrengths = Enumerable.Range(0, 6).Select(x => x * 40 + 20)
	.Select(c => cycles.First(x => x.cycle == c).reg * c);

Console.WriteLine(signalStrengths.Sum());

// Part 2
var output = cycles.Aggregate(new System.Text.StringBuilder(), (sb, c) =>
{
	var position = c.cycle % 40;
	sb.Append(c.reg >= position - 2 && c.reg <= position ? "#" : ".");
	if (position == 0)
	{
		sb.Append("\n");
	}
	return sb;
});

Console.WriteLine(output);

IOp CreateOp(string s)
{
	var pair = s.Split(" ", StringSplitOptions.RemoveEmptyEntries);
	return pair.First() switch
	{
		"noop" => new NoOp(),
		"addx" => new Add(int.Parse(pair.Last())),
		_ => throw new ArgumentException()
	};
}

interface IOp
{
	int Cycles { get; }
	int Operate(int arg);
}

class NoOp : IOp
{
	public int Cycles => 1;
	public int Operate(int arg) => arg;
}

class Add : IOp
{
	readonly int value;

	public Add(int value) =>
		this.value = value;

	public int Cycles => 2;
	public int Operate(int arg) => arg + value;
}