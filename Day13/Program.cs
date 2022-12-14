var chars = System.IO.File.ReadAllText("data.txt")
	.Split(string.Concat(Environment.NewLine, Environment.NewLine))
	.Select(x => x.Split(Environment.NewLine))
	.Select(x => (l: x.First().ToCharArray(), r: x.Last().ToCharArray()))
	.ToList();

IEnumerable<Pair> GetPairs(IEnumerable<(char [], char [])> data) =>
	data.Select(x => 
	{
		var l = CreateDirectory(x.Item1);
		var r = CreateDirectory(x.Item2);

		return new Pair(l, r);
	});

Directory CreateDirectory(char [] chars)
{
	var dir = new Directory();
	var numBuffer = new List<char>();

	foreach (var c in chars.Skip(1))
	{
		if (Char.IsDigit(c))
		{
			numBuffer.Add(c);
			continue;
		}

		if ((c == ']' || c == ',') && numBuffer.Count() > 0)
		{
			var str = new string(numBuffer.ToArray());
			var num = int.Parse(str);
			dir.Items.Add(new Number(num));
			numBuffer.Clear();
		}

		if (c == '[')
		{
			var newDirectory = new Directory(dir);
			dir.Items.Add(newDirectory);
			dir = newDirectory;
		}
		else if (c == ']')
		{
			if (dir.Parent == null)
				return dir;
			dir = dir.Parent!;
		}
	}

	throw new ArgumentException("No ending ']'");
}

var pairs = GetPairs(
	chars.Concat(
		new [] { (l: "[[2]]".ToCharArray(), r: "[[6]]".ToCharArray()) }));

var ordered = pairs.SelectMany(x => new [] { x.l , x.r })
	.Order(new DirectoryComparer()).ToList();

var indexes = ordered.Select((x, i) => (i+1, x))
	.Where(x => x.x.Output() == "[[2]]" || x.x.Output() == "[[6]]")
	.Select(x => (index: x.Item1, x));

	var product = indexes.Aggregate(1, (agg, x) => agg * x.index);

Console.WriteLine(product);

/*foreach (var a in indexes)
	Console.WriteLine($"{a.Item1} {a.x.x.Output()}");

foreach (var p in pairs)
{
	Console.WriteLine(p.l.Output());
	Console.WriteLine(p.r.Output());
	Console.WriteLine();
}*/

abstract class Item
{
	public abstract string Output();
}

class Number : Item
{
	public Number(int value) =>
		Value = value;

	public int Value { get; }

  public override string Output() =>
		Value.ToString();
}

class Directory : Item
{
	public Directory() {}

	public Directory(Directory parent) =>
		Parent = parent;

	public Directory(Number num) =>
		Items.Add(num);

	public Directory? Parent { get; } 
	public IList<Item> Items { get; } = new List<Item>();

  public override string Output() =>
		$"[{string.Join(",", Items.Select(x => x.Output()))}]";
}

record Pair(Directory l, Directory r);

class DirectoryComparer : IComparer<Directory>
{
  public int Compare(Directory? x, Directory? y)
  {
			var dirCompare = Compare0(x!, y!);
			return dirCompare == null || dirCompare == true ? -1 : 1;
  }

  bool? Compare0(Directory l, Directory r)
	{
		for (var i = 0; i < l.Items.Count(); i++)
		{
			if (i == r.Items.Count())
				return false;

			var lItem = l.Items[i];
			var rItem = r.Items[i];

			if (lItem is Number l0 && rItem is Number r0)
			{
				if (l0.Value < r0.Value)
					return true;
				else if (l0.Value > r0.Value)
					return false;
			}
			else
			{
				var ld = lItem is Directory ? (Directory)lItem : new Directory((Number)lItem);
				var rd = rItem is Directory ? (Directory)rItem : new Directory((Number)rItem);

				var dirCompare = Compare0(ld, rd);

				if (dirCompare == true)
					return true;
				else if (dirCompare == false)
					return false;
			}
		}
		return l.Items.Count() < r.Items.Count() ? true : null;
	}
}