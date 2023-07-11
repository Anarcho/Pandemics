using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Pandemics
{
    public class MainTabWindow_Pandemics : MainTabWindow
    {
        public const int FreeSpaceAtTheEnd = 50;
        public const float ButtonWidth = 160f;
        public const float RowHeight = 35f;
        protected const float ColumnWidth = 175f;
        protected const float LeftMargin = 15f;
        protected Vector2 scrollPosition = Vector2.zero;
        protected Dictionary<string, VirusRecord> virusRecords = VirusManager.virusRecords;
        public float desiredWidth = 0f;

        public override Vector2 InitialSize => new Vector2(600f, 400f);

        public override void DoWindowContents(Rect canvas)
        {
            Rect titleRect = new Rect(
                canvas.xMin,
                canvas.yMin,
                canvas.width,
                30f
            );

            Widgets.Label(titleRect, "Pandemics");

            Rect contentRect = new Rect(
                canvas.xMin,
                titleRect.yMax,
                canvas.width,
                canvas.height - titleRect.height
            );

            DrawPawnsWithViruses(contentRect);
        }

        private void DrawPawnsWithViruses(Rect rect)
        {
            float x = rect.xMin;
            float y = rect.yMin;

            // Headers
            Rect virusNameHeaderRect = new Rect(x, y, ColumnWidth, RowHeight);
            Widgets.Label(virusNameHeaderRect, new GUIContent("Virus Name"));
            x += ColumnWidth;

            Rect virusDefNameHeaderRect = new Rect(x, y, ColumnWidth, RowHeight);
            Widgets.Label(virusDefNameHeaderRect, new GUIContent("Virus DefName"));
            x += ColumnWidth;

            Rect colonistHeaderRec = new Rect(x, y, ColumnWidth, RowHeight);
            Widgets.Label(colonistHeaderRec, new GUIContent("Colonist"));
            x = rect.xMin;
            y += RowHeight;

            // Data rows
            foreach (KeyValuePair<string, VirusRecord> virusPair in virusRecords)
            {
                Rect rowRect = new Rect(x, y, ColumnWidth * 3, RowHeight * (virusPair.Value.InfectedPawns.Count + 1));

                // Virus Name column
                Rect virusNameRect = new Rect(rowRect.x, rowRect.y, ColumnWidth, RowHeight);
                Widgets.Label(virusNameRect, new GUIContent(virusPair.Key));

                // Virus DefName column
                Rect virusDefNameRect = new Rect(rowRect.x + ColumnWidth, rowRect.y, ColumnWidth, RowHeight);
                string virusDefName = virusPair.Value.VirusDefName;
                Widgets.Label(virusDefNameRect, new GUIContent(virusDefName));

                // Colonist column
                if (virusPair.Value.InfectedPawns.Count > 0)
                {
                    for (int i = 0; i < virusPair.Value.InfectedPawns.Count; i++)
                    {
                        Rect pawnRect = new Rect(rowRect.x + (ColumnWidth * 2), rowRect.y + (RowHeight * i), ColumnWidth, RowHeight);
                        Widgets.Label(pawnRect, new GUIContent(virusPair.Value.InfectedPawns[i].Name.ToStringShort));
                    }
                }

                x = rect.xMin;
                y += rowRect.height;
            }
        }

    }
}
