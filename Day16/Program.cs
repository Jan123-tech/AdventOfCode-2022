var forces = System.IO.File.ReadAllText("data.txt").ToCharArray();

foreach (var item in valves0)
{
	foreach (var id in item.valves)
	{
		item.v.Valves.Add(valves0.First(x => x.v.Id == id).v);
	}
}

var valves = valves0.Select(x => x.v);
var valvesWorking = valves.Where(x => x.IsWorking).ToArray();

foreach (var v in valvesWorking)
{
	v.BuildSteps();
}

foreach (var v in valvesWorking)
foreach (var r in v.Steps.Where(x => x.Key.IsWorking).Select(x => x.Key))
Console.WriteLine($"{v.Id} -> {r.Id}: {v.Steps[r]}");

var length = valvesWorking.Count();
var paths = new List<List<Valve>>()
	.Concat(Enumerable.Range(1, length).Select(x => new List<Valve>())).ToArray();

var combos = valvesWorking.SelectMany(x => valvesWorking.Select(x0 => (x, x0))).Where(x => x.x != x.x0).ToList();

//foreach (var c in combos)

//Console.WriteLine($"{c.Item1.Id} -> {c.Item2.Id}");


 

class Valve
{
	public Valve(string id, int rate)
	{
		Rate = rate;
		Id = id;
	}

	public string Id { get; }
	public int Rate { get; }

	public IDictionary<Valve, int> Steps = new Dictionary<Valve, int>();

	public void BuildSteps()
	{
		var index = 0;
		Steps = new Dictionary<Valve, int> { { this, index } } ;
		while (true)
		{
			var items = Steps.Where(x => x.Value == index).Select(x => x.Key).ToList();
			if (!items.Any())
				break;
			index++;
			foreach (var v in items.SelectMany(v => v.Valves))
			{				
				if (!Steps.ContainsKey(v))
					Steps.Add(v, index);
			}
		}
	}

	public bool IsWorking => Rate > 0;

	public IList<Valve> Valves { get; } = new List<Valve>();

  public string Output() =>
		$"Valve {Id} has flow rate={Rate}; tunnels lead to valves {string.Join(", ", Valves.Select(x => x.Id))}";
}