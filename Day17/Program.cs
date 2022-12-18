var forces = System.IO.File.ReadAllText("data.txt").ToCharArray();

var bufferService = new BufferService();
var shapeFactory = new ShapeFactory(bufferService);

var screenBuffer = bufferService.GetBuffer(9, 8100);
bufferService.Copy("+-------+", screenBuffer, 0, 0);
for (var y = 1; y < screenBuffer.GetLength(1); y++)
	bufferService.Copy("|.......|", screenBuffer, 0, y);

var forceIndex = 0;
var shapeCount = 0;
var maxShapes = 2022;
var highestY = 0;
bool freezeChar = true;

var start = DateTime.Now;
Console.WriteLine(start);
long i = 0;
for (i = 0; i < 100000000000; i++)
	i++;

Console.WriteLine(DateTime.Now - start);

while (shapeCount < maxShapes)
{
	var shape = shapeFactory.Create();
	shape.y = highestY + 4;

	shapeCount++;

	while (true)
  {
    var force = GetForce(forces, ref forceIndex);

    if (!IsCollision(shape, screenBuffer, force, 0))
      shape.x += force;

    if (IsCollision(shape, screenBuffer, 0, -1))
      break;

    shape.y--;
  }

  freezeChar = !freezeChar;
	bufferService.Copy(shape.Bitmap, screenBuffer, shape.x, shape.y, freezeChar ? '#' : '@');
	highestY = HighestY(screenBuffer);
}

static int GetForce(char[] forces, ref int forceIndex)
{
  var force = forces[forceIndex] == '>' ? 1 : -1;
  forceIndex++;
  if (forceIndex == forces.Length)
    forceIndex = 0;
  return force;
}

int HighestY(char[,] buffer)
{
	for (var y = buffer.GetLength(1) - 1; y >= 0 ; y--)
		for (var x = 1; x < buffer.GetLength(0) - 1; x++)
		{
			var c = buffer[x, y];
			
			if (c != '.')
				return y;
		}
	return 0;
}

bool IsCollision(Shape shape, char[,] buffer, int xDir, int yDir)
{
	for (var x = 0; x < shape.Bitmap.GetLength(0); x++)
		for (var y = 0; y < shape.Bitmap.GetLength(1); y++)
		{
			var xB = x + shape.x + xDir;
			var yB = y + shape.y + yDir;

			var shapeC = shape.Bitmap[x, y];
			var bufferC = buffer[xB, yB];
			
			var isCollision = shapeC != '.' && bufferC != '.';

			if (isCollision)
				return true;
		}
	return false;
}

bufferService.Output(screenBuffer);

Console.WriteLine(highestY);



//Output(buffer);