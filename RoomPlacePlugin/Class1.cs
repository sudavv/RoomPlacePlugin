using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomPlacePlugin
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class PlaceRoom : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            CreateRooms(doc);
            return Result.Succeeded;
        }

        private void CreateRooms(Document document)
        {
            PhaseArray phases = document.Phases;
            Phase createRoomsInPhase = phases.get_Item(phases.Size - 1);
            FilteredElementCollector collector = new FilteredElementCollector(document);
            collector.OfClass(typeof(Level));

            Transaction transaction = new Transaction(document);                 
            transaction.Start("Создание помещений");
            int x = 1;
            foreach (Level level in collector)
                {
                    string numlvl = level.Name.Substring(level.Name.Length-1);
                    PlanTopology topology = document.get_PlanTopology(level, createRoomsInPhase);
                    PlanCircuitSet circuitSet = topology.Circuits;
                    foreach (PlanCircuit circuit in circuitSet)
                    {
                        if (!circuit.IsRoomLocated)
                        {
                            Room room = document.Create.NewRoom(null, circuit);
                            room.Name = numlvl + "_" + x;
                            x++;
                        }
                    }
                }
            transaction.Commit();
            }
        }
    }

