﻿using SmartFormat;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace VisualSAIStudio
{
    public abstract class SmartAction : SmartElement
    {
        private string readable;
        public string name;
        private SmartTarget _target;
        public SmartTarget target
        { 
            get
            {
                return this._target;
            }
            set
            {
                this._target = value;
                paramValueChanged(this, new EventArgs());
            }
        }

        public SmartAction() : base()
        {
            target = new SMART_TARGET_SELF();
        }

        public SmartAction(int id, string name) : base()
        {
            target = new SMART_TARGET_SELF();
            this.ID = id;
            this.name = name;
        }

        protected virtual string GetCpp() { return ""; }

        public string GetCPPCode()
        {
            string code = Smart.Format(GetCpp(), new
            {
                target_creature = "{target_creature}",
                pram1 = parameters[0],
                pram2 = parameters[1],
                pram3 = parameters[2],
                pram4 = parameters[3],
                pram5 = parameters[4],
                pram6 = parameters[5]
            });
            StringBuilder ret = new StringBuilder();
                ret.AppendLine(target.GetCPPCode());
                ret.AppendLine("for (ObjectList::const_iterator itr = targets->begin(); itr != targets->end(); ++itr)");
                ret.AppendLine("{");
            if (code.Contains("{target_creature}"))
            {
                ret.AppendLine("if (Creature* creature = (*itr)->ToCreature())");
                ret.AppendLine(code.Replace("{target_creature}", "creature"));
            }
            else
                ret.AppendLine(code);
            ret.AppendLine("}");
            return ret.ToString();
        }

        public override void Copy(SmartElement prev)
        {
            base.Copy(prev);
            this.target = ((SmartAction)prev).target;
        }

        public override Size Draw(Graphics graphics, int x, int y, int width, int height, Brush brush, Pen pen, Font font, bool setRect = true)
        {
            SizeF size = graphics.MeasureString(ToString(), font);

            brush = Brushes.Black;
            graphics.DrawString(ToString(), font, brush, x + 5, y + 3);

            if (setRect)
                SetRect(x, y, width, (int)size.Height + 6);

            if (selected)
                graphics.DrawLine(pen, x, y, x, y + size.Height);

            pen.Color = Color.Black;
            return new Size(width, (int)size.Height+6);
        }

        protected override void paramValueChanged(object sender, EventArgs e)
        {
            if (GetReadableString() == null)
                return;
            readable = Smart.Format(GetReadableString(), new { target = target.GetReadableString(),
                                                               targetcoords = target.GetCoords(), 
                                                               targetid = target.ID,
                                                               pram1 = parameters[0],
                                                               pram2 = parameters[1],
                                                               pram3 = parameters[2],
                                                               pram4 = parameters[3],
                                                               pram5 = parameters[4],
                                                               pram6 = parameters[5],
                                                               pram1value = parameters[0].GetValue(),
                                                               pram2value = parameters[1].GetValue(),
                                                               pram3value = parameters[2].GetValue(),
                                                               pram4value = parameters[3].GetValue(),
                                                               pram5value = parameters[4].GetValue(),
                                                               pram6value = parameters[5].GetValue()});
            base.paramValueChanged(sender, e);
        }

        protected override void UpdateParamsInternal(int index, int value)
        {
            this.parameters[index].SetValue(value);
            paramValueChanged(this, new EventArgs());
        }

        public override string ToString()
        {
            return readable;
        }
    }

}