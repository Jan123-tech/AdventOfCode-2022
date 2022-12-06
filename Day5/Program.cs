var stacks = File.ReadAllLines("data.txt")
	.TakeWhile(x => x.IndexOf("[") != -1)
	.Select(x => x
		.Chunk(4)
		.Select(x => new string(x))
		.Select((x, colIndex) =>
			(colIndex, crate: string.Join(string.Empty, x.Split('[', ']')).Trim()))
		.Where(x => x.crate != string.Empty))
	.Reverse()
	.SelectMany(x => x)
	.Aggregate(new List<Stack<string>>(), (list, x) =>
	{
		if (list.Count() < x.colIndex + 1)
			list.Add(new Stack<string>());

		list[x.colIndex].Push(x.crate);

		return list;
	});

var instructions = File.ReadAllLines("data.txt")
	.SkipWhile(x => !x.StartsWith("move"))
	.Select(x => x
		.Replace("move ", string.Empty)
		.Replace(" from", string.Empty)
		.Replace(" to", string.Empty)
		.Split(" ")
		.Select(x => int.Parse(x))
		.ToList())
	.Select(x => new Instruction(x[0], x[1] - 1, x[2] - 1));

foreach (var inst in instructions)
{
	var items = new Stack<string>();

	for (var i = 0; i < inst.count; i++)
		items.Push(stacks[inst.source].Pop());

	foreach (var item in items)
		stacks[inst.target].Push(item);             
}

var tops = stacks.Aggregate(new System.Text.StringBuilder(), (current, s) =>
	current.Append(s.Peek()));

Console.WriteLine(tops);

record Instruction(int count, int source, int target);
