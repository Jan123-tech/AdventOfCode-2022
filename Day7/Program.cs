var commands = System.IO.File.ReadAllText("data.txt")
	.Split("$ ", StringSplitOptions.RemoveEmptyEntries)
	.Select(x => x.Trim('\n'))
	.Skip(1);

var root = new Directory("/", null);
var dir = root;

foreach (var c in commands)
{
	if (c.StartsWith("cd"))
	{
		var val = c.Substring(c.IndexOf(" ") + 1);
		dir = val == ".." ?
			dir.Parent! :
			(Directory) dir.Items.First(x => x.Name == val);
	}
	else
	{
		var val = c.Substring(c.IndexOf('\n') + 1);
		foreach (var i in val.Split('\n'))
		{
			var name = i.Substring(i.LastIndexOf(" ") + 1);
			dir.Items.Add(i.StartsWith("dir") ?
				new Directory(name, dir) :
				new File(name, int.Parse(i.Substring(0, i.IndexOf(" ")))));
		}
	}
}

var totalSpace = 70000000;
var spaceRequired = 30000000;
var spaceUsed = root.Size; // 46552309
var spaceRemaining = totalSpace - spaceUsed; // 23447691
var spaceToFree = spaceRequired - spaceRemaining; // 6552309

Console.WriteLine(root.AllDirectories
	.Where(d => d.Size >= spaceToFree)
	.OrderBy(x => x.Size)
	.First()
	.Size);

abstract class Item
{
	public Item(string name) =>
		Name = name;

	public string Name { get; }
	public abstract int Size { get; }
}

class File : Item
{
	public File(string name, int size) : base(name) =>
		Size = size;

	public override int Size { get; }
}

class Directory : Item
{
	public Directory(string name, Directory? parent) : base(name) =>
		Parent = parent;

	public override int Size
	{
		get => Items.Sum(x => x.Size);
	}

	public Directory? Parent { get; } 
	public IList<Item> Items { get; } = new List<Item>();
	public IEnumerable<Directory> AllDirectories =>
		Items.OfType<Directory>()
			.SelectMany(x => new [] { x }.Concat(x.AllDirectories));
}