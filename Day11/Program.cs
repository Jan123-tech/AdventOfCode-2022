// Part 1 ONLY
var monkeys = File.ReadAllText("data.txt").Split("\n\n")
	.Aggregate(new List<Monkey>(), (monkeys, s) =>
		monkeys.Concat(new [] { CreateMonkey(s) }).ToList());

var counts = Enumerable.Range(1, monkeys.Count()).Select(x => 0).ToArray();

foreach (var round in Enumerable.Range(1, 20))
{
	for (var i = 0; i < monkeys.Count(); i++)
	{
		var m = monkeys[i];

		counts[i] += m.Items.Count();

		var result = m.Go();

		foreach (var r in result)
		{
			var m0 = monkeys[r.index];
			m0.Items.Add(r.item);
		}	
	}
}

Console.WriteLine(counts.OrderByDescending(x => x).Take(2)
	.Aggregate(1, (agg, x) => agg * x));

Monkey CreateMonkey(string s)
{
	var lines = s.Split("\n");

	var ops = lines[2].Substring(lines[2].LastIndexOf(" ") - 1).Split(" ");
	Func<int, int> op = ops.Last() == "old" ?
		x => x * x :
		ops.First() == "*" ?
			x => x * (int.Parse(ops[1])) :
			x => x + (int.Parse(ops[1]));

	int GetNumberAtEnd(string s) =>
		int.Parse(s.Substring(s.LastIndexOf(" ")));

	var m = new Monkey(
		lines[1]
			.Substring(lines[1].IndexOf(":") + 2)
			.Split(",")
			.Select(x => int.Parse(x.Trim()))
			.ToList(),
		op,
		GetNumberAtEnd(lines[3]),
		GetNumberAtEnd(lines[4]),
		GetNumberAtEnd(lines[5]));

	return m;
}

record Monkey(IList<int> Items, Func<int, int> Op,
	int Divisor, int IfTrue, int IfFalse)
{
	public IEnumerable<(int item, int index)> Go() =>
		Items.Select(x => x).ToList()
			.Aggregate(new List<(int, int)>(), (agg, item) =>
			{
				var newItem = Op(item) / 3;
				var index = (newItem % Divisor) == 0 ? IfTrue : IfFalse;
				Items.Remove(item);
				agg.Add((newItem, index));
				return agg;
			});
};