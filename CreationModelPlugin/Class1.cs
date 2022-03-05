using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreationModelPlugin
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class CreationModelPlugin : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Level level1 = LevelUtils.SelectLevel(doc, "Этаж 1");

            Level level2 = LevelUtils.SelectLevel(doc, "Этаж 2");

            WallUtils.WallCreate(doc, 10000, 5000, level1, level2);

            
            return Result.Succeeded;
        }
    }
    public class LevelUtils
    {
        public static Level SelectLevel (Document doc, string name)
        {
            Level level = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()                
                .Where(x => x.Name.Equals(name))
                .FirstOrDefault();
            return level;
        }
    }
    public class WallUtils
    {
        public static void WallCreate (Document doc, int width, int depth, Level level1, Level level2)
        {
            double width1 = UnitUtils.ConvertToInternalUnits(width, DisplayUnitType.DUT_MILLIMETERS);
            double depth1 = UnitUtils.ConvertToInternalUnits(depth, DisplayUnitType.DUT_MILLIMETERS);
            double dx = width1 / 2;
            double dy = depth1 / 2;

            List<XYZ> points = new List<XYZ>();
            points.Add(new XYZ(-dx, -dy, 0));
            points.Add(new XYZ(dx, -dy, 0));
            points.Add(new XYZ(dx, dy, 0));
            points.Add(new XYZ(-dx, dy, 0));
            points.Add(new XYZ(-dx, -dy, 0));

            List<Wall> walls = new List<Wall>();

            Transaction transaction = new Transaction(doc, "Построение стен");
            transaction.Start();
            for (int i = 0; i < 4; i++)
            {
                Line line = Line.CreateBound(points[i], points[i + 1]);
                Wall wall = Wall.Create(doc, line, level1.Id, false);
                walls.Add(wall);
                wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level2.Id);
            }
            transaction.Commit();
        }

    }
}
