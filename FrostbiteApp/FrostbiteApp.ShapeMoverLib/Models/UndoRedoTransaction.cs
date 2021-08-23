using FrostbiteApp.ShapeMoverLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrostbiteApp.ShapeMoverLib.Models
{
    public class UndoRedoTransaction
    {
        public ShapeModel Data { get; set; }
        public Point OldValue { get; set; }
        public Point NewValue { get; set; }

        public UndoRedoReason Reason { get; set; }

        public Point GetValue(bool isUndo)
        {
            if (isUndo)
                return OldValue;

            return NewValue;
        }
    }
}
