# GraphicBox2D ReadMe

![GraphicBox2D Demo](https://raw.githubusercontent.com/kanineko17/GraphicControl2D/main/README2.png)

## GitHub Repository
https://github.com/kanineko17/GraphicControl2D

## Contact information
If you find any bugs or have feature requests, please feel free to contact me here! 「info@kanineko.com」

## What's New Version
-  (v3.2.3) Added Group Shape Objects and Layer functionality.
-  (v3.2.2)Fixed an issue where the Y-axis grid labels had inverted signs.
-  (v3.2.0)Fixed minor bugs and Updated README
-  (v3.1.4)Added background image support.Adjusted the grid line width to be thinner.
-  (v3.0.0)Changed the rendering engine to SkiaSharp.

## English

### Usage

1. After installation, Drag **"Graphic2DControl"** from the Toolbox and drop it onto your form.

2. Use the following sample code to draw shapes inside the `Graphic2DControl`.

![GraphicBox2D Demo](https://raw.githubusercontent.com/kanineko17/GraphicControl2D/main/README4.png)

```csharp
Layer2D layer2D = new Layer2D();
layer2D.LayerName = "Layer1";
layer2D.ZOrder = 0;
layer2D.IsVisible = true;

this.graphic2DControl1.Layers.Add(layer2D);

// Point
Point2D point = new Point2D();
point.X = -1;
point.Y = -1;
this.graphic2DControl1.Layers[0].Points.Add(point);

// Circle
Circle2D circle = new Circle2D();
circle.X = 3;
circle.Y = 3;
circle.R = 1.0f;
circle.IsFilled = false;
circle.LineColor = Color.Red;
this.graphic2DControl1.Layers[0].Circles.Add(circle);

// Polygon
Polygon2D polygon2D = new Polygon2D();
polygon2D.LineColor = Color.Yellow;
polygon2D.LineStyle = LineStyle.Solid;
polygon2D.IsFilled = true;
polygon2D.FillColor = Color.Green;
polygon2D.Points.Add(new PointF(-2, 2));
polygon2D.Points.Add(new PointF(-1, 4));
polygon2D.Points.Add(new PointF(0, 2));
this.graphic2DControl1.Layers[0].Polygons.Add(polygon2D);

// Text : Welcome
Text2D text = new Text2D();
text.X = -3;
text.Y = -3;
text.FontSize = 16.0f;
text.Text = "Welcome to the graphicBox2d";
text.Angle = 30.0f;
this.graphic2DControl1.Layers[0].Texts.Add(text);

Group2D group = new Group2D();

// Arrow
Arrow2D arrow = new Arrow2D();
arrow.Start = new Point(0, 0);
arrow.End = new Point(3, 3);
arrow.Color = Color.Green;
arrow.Style = LineStyle.Solid;
group.ObjectList.Add(new Group2DItem(arrow, 0));

// Text : Angle
Text2D text2 = new Text2D();
text2.X = 0.4f;
text2.Y = 0.4f;
text2.FontSize = 11.0f;
text2.Text = "45°";
text2.Color = Color.Cyan;
group.ObjectList.Add(new Group2DItem(text2, 1));

// Arc
Arc2D arc = new Arc2D();
arc.X = 0;
arc.Y = 0;
arc.R = 1.0f;
arc.StartAngle = 0f;
arc.EndAngle = 45.0f;
arc.IsFilled = false;
arc.LineColor = Color.Cyan;
group.ObjectList.Add(new Group2DItem(arc, 2));

this.graphic2DControl1.Layers[0].Groups.Add(group);

// Text : cosθ
Text2D text3 = new Text2D();
text3.X = 1.5f;
text3.Y = 1.0f;
text3.FontSize = 16.0f;
text3.Text = "cosθ";
this.graphic2DControl1.Layers[0].Texts.Add(text3);

// Graph
MathGraph2D graph = new MathGraph2D();
graph.Susiki = "cos(x)";
graph.StartX = -50.0f;
graph.EndX = 50.0f;
graph.Color = Color.White;
graph.CalculateInterval = 0.05f;
graph.CalculateGraphPoints();
this.graphic2DControl1.Layers[0].MathGraphs.Add(graph);

// Redraw
this.graphic2DControl1.Invalidate();
```
![GraphicBox2D Demo](https://raw.githubusercontent.com/kanineko17/GraphicControl2D/main/README_PNG/README3.png)

### Mode
You can switch between a simple drawing mode and an interactive mode that allows mouse operations.

- **Default Mode**: A basic drawing mode. Mouse interactions are not supported.
- **Slect Mode**: An interactive mode that allows you to select, move, and delete shape objects using the mouse.

![GraphicBox2D Demo](https://raw.githubusercontent.com/kanineko17/GraphicControl2D/main/README_PNG/README5.png)

### データの保存

You can save the drawn shape objects in JSON format.
		
```csharp
// Save
this.graphic2DControl1.SaveData(@"C:\Users\kani\Desktop\Data\shapes.json");
```

### データの読み込み
You can load shape object data from a previously saved JSON file
		
```csharp
// Load
this.graphic2DControl1.LoadData(@"C:\Users\kani\Desktop\Data\shapes.json");
```