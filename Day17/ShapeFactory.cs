class ShapeFactory
{
	IBuffer bufferService;

	public ShapeFactory(IBuffer buffer) =>
    this.bufferService = buffer;

	int index = 0;

	public Shape Create()
	{
		if (index == Shapes.Length)
			index = 0;

    var shapeStr = Shapes[index++];
		var shape = bufferService.Create(shapeStr);
    var startOffset = shapeStr.Split(Environment.NewLine).Min(x => x.IndexOf("@"));

		return new Shape(shape, startOffset+3, 50, startOffset);
	}
  	string[] Shapes = new string[] { "@@@@",
@".@.
@@@
.@.",

@"@@@
..@
..@",

@"@
@
@
@",

@"@@
@@" };
}