var test = 1 == 0;
var file = $"data{(test ? ".test" : string.Empty)}.txt";
var xS = test ? 5 : 68;
var yS = test ? 2 : 20;

var rows = File.ReadAllLines(file);
var map = ParseMap(rows);

var visited = new HashSet<Position>
{
    new(xS, yS)
};

int count = 0;
var queue = new Queue<Path>();
queue.Enqueue(new Path(new Position(xS, yS), 0, null));

Path? end = null;

while (queue.Any())
{
    count++;
    var item = queue.Dequeue();
    var cellValue = map[item.Position.X, item.Position.Y];

    Console.WriteLine($"{count}: Char: {cellValue} Count: {item.Step} Point: {item.Position}");

    if (cellValue == 'a')
    {
        end = item;
        break;
    }

    var nextPoints0 = new List<Position>
    {
        new (item.Position.X + 1, item.Position.Y), new (item.Position.X - 1, item.Position.Y),
        new (item.Position.X, item.Position.Y + 1), new (item.Position.X, item.Position.Y - 1)
    };

    var nextPoints1 = nextPoints0.Where(x => x.X >= 0 && x.X < map.GetLength(0) && x.Y >= 0 && x.Y < map.GetLength(1));
    var nextPoints2 = nextPoints1.Where(x => !visited.Contains(x));
    var nextPoints3 = nextPoints2.Where(x =>
    {
        var newCellValue = map[x.X, x.Y];
        var isDown = newCellValue == cellValue - 1;
        var isSameOrUp = newCellValue >= cellValue;
        return isDown || (isSameOrUp && cellValue != 'E') || (cellValue == 'E' && newCellValue == 'z');
    });

    foreach (var i in nextPoints3)
    {
        visited.Add(i);
        queue.Enqueue(new (i, item.Step + 1, item));
    }
}



var endPath = new HashSet<Position>();
var tempPath = end!;
while (tempPath.Parent != null)
{
    endPath.Add(tempPath.Position);
    tempPath = tempPath.Parent;
}
Output();

void Output()
{
    for (var i = 0; i < map.GetLength(1); i++)
    {
        for (var j = 0; j < map.GetLength(0); j++)
        {
            var c = ((map[j,i] == 'E') ? 'E' : map[j,i]);
            if (endPath.Contains(new Position(j, i)))
             c = '0';
            Console.Write(c);
        }
        Console.Write("\n");
    }
}

static char[,] ParseMap(string[] rows)
{
  var map = new char[rows[0].Length, rows.Length];

  for (int r = 0; r < rows.Length; r++)
  {
    var cells = rows[r].ToCharArray();

    for (int c = 0; c < cells.Length; c++)
    {
      map[c, r] = cells[c];
    }
  }

  return map;
}

record Position(int X, int Y);

record Path(Position Position, int Step, Path? Parent);

