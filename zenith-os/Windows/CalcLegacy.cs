
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.Drawing;
using zenithos.Controls;

///
/// This Calculator app is legacy, look Windows/Calc.cs instead with a better expression parser
///

namespace zenithos.Windows
{
    internal class CalcLegacy : Window
    {
        public List<List<Button>> buttons = new();

        public List<Button> additionalButtons = new();
        public Button plusButton, minusButton, mulButton, divButton,eqButton,cButton,expandButton,sqrtButton;
        public Label result;
        string firstNumber, secondNumber;
        string operation = null;
        bool isModifyingSecondField = false;
        bool expanded = false;

        [ManifestResourceStream(ResourceName = "zenithos.Resource.Applogos.calc.bmp")]
        readonly static byte[] logoBytes;


        public CalcLegacy() : base(100, 100, 240, 270, "Calculator", Kernel.defFont)
        {
            logo =  new Bitmap(logoBytes);
            int e = 1;
            for(int i =0;i < 3; i++)
            {
                List<Button> cur = new List<Button>();
                for (int j =0;j < 3; j++)
                {
                    Button nw = new(e.ToString(), 20+j * 50, 50+i * 50, Kernel.mainCol, Kernel.defFont, 15);
                    controls.Add(nw);
                    cur.Add(nw);
                    e++;
                }
                buttons.Add(cur);
            }
            Button zeroButton = new Button("0",70,200,Kernel.mainCol,Kernel.defFont,15);
            controls.Add(zeroButton);
            List<Button> last = new()
            {
              zeroButton
            };
            buttons.Add(last);
            plusButton = new Button("+",170,50,Kernel.mainCol, Kernel.defFont, 15);
            minusButton = new Button("-",170,100,Kernel.mainCol,Kernel.defFont,15);
            mulButton = new Button("*", 170, 150,Kernel.mainCol, Kernel.defFont,15);
            divButton = new Button("/", 170, 200,Kernel.mainCol,Kernel.defFont,15);
            expandButton = new Button(">",190,20,Kernel.mainCol,Kernel.defFont,5);
            eqButton = new Button("=",120,200,Kernel.mainCol,Kernel.defFont,15);
            cButton = new Button("C",20,200,Kernel.mainCol,Kernel.defFont,15);
            result = new Label("", 20, 20, Kernel.defFont, Kernel.textColDark);
            sqrtButton = new Button("sqrt",220,50,Kernel.mainCol,Kernel.defFont,15);
            controls.Add(sqrtButton);
            additionalButtons.Add(sqrtButton);
            controls.Add(plusButton);
            controls.Add(minusButton);
            controls.Add(mulButton);
            controls.Add(divButton);
            controls.Add(eqButton);
            controls.Add(cButton);
            controls.Add(result);
            controls.Add(expandButton);
            foreach (Button b in additionalButtons)
            {
                b.Visible = false;
            }
        }
        public override void Update(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            try {
                
               base.Update(canv, mX, mY, mD, dmX, dmY);
           
               for (int i = 0; i < buttons.Count; i++)
               {
                   for (int j = 0; j < buttons[i].Count; j++)
                   {
                       if (buttons[i][j].clickedOnce)
                       {
                           if (isModifyingSecondField)
                           {
                               secondNumber += buttons[i][j].Text;
                           }
                           else
                           {
                               firstNumber += buttons[i][j].Text;
                           }
                       }
                   }
               }
           
           if (expandButton.clickedOnce)
           {
               expanded = !expanded;
               if (expanded)
               {
                   w = 240 + 50;
                   expandButton.Text = "<";
                   foreach (Button b in additionalButtons)
                   {
                       b.Visible = true;
                   }
               }
               else
               {
                   w = 240;
                   expandButton.Text = ">";
                   foreach (Button b in additionalButtons)
                   {
                       b.Visible = false;
                   }
               }
           }

           
           result.Text = isModifyingSecondField?$"{firstNumber} {operation} {secondNumber}":firstNumber;
          
          
           if (sqrtButton.clickedOnce)
           {
               if (isModifyingSecondField)
               {
                        if (!String.IsNullOrEmpty(secondNumber) && secondNumber != "-")
                        {
                            string last = secondNumber;
                            double sqrt = Math.Sqrt(Convert.ToDouble(secondNumber));
                            if (double.IsNaN(sqrt))
                            {
                                Kernel.ShowMessage("Don't try doing this, you might break something...", "Real Numbers", MsgType.Info);
                                secondNumber = last;
                            }
                            else
                            {
                                secondNumber = sqrt.ToString();
                            }
                        }
               }
               else
               {
                        if (!String.IsNullOrEmpty(firstNumber) && firstNumber != "-")
                        {
                            string last = firstNumber;
                            double sqrt = Math.Sqrt(Convert.ToDouble(firstNumber));
                            if (double.IsNaN(sqrt))
                            {
                                Kernel.ShowMessage("Don't try doing this, you might break something...", "Real Numbers", MsgType.Info);
                                firstNumber = last;
                            }
                            else
                            {
                                firstNumber = sqrt.ToString();
                            }
                        }
               }
           }

           if (cButton.clickedOnce)
           {
               firstNumber = "";
               secondNumber = "";
               operation = null;
               isModifyingSecondField = false;
           }

           if (plusButton.clickedOnce)
           {
               if (!String.IsNullOrEmpty(firstNumber) && firstNumber != "-")
               {
                        operation = "+";
                        isModifyingSecondField = true;
               }
           }

           if (minusButton.clickedOnce)
           {
                if (String.IsNullOrEmpty(firstNumber))
                {
                    firstNumber = "-";    
                }
                else
                {
                    operation = "-";
                    isModifyingSecondField = true;
                }
           }

           if (mulButton.clickedOnce)
           {
                if (!String.IsNullOrEmpty(firstNumber) && firstNumber != "-")
                {
                        operation = "*";
                        isModifyingSecondField = true;
                }
           }
           if (divButton.clickedOnce)
           {
               if (!String.IsNullOrEmpty(firstNumber) && firstNumber != "-")
               {
                        operation = "/";
                        isModifyingSecondField = true;
               }
           }

           if (eqButton.clickedOnce)
           {
               if (operation != null && !String.IsNullOrEmpty(secondNumber) && !String.IsNullOrEmpty(firstNumber) && secondNumber!= "-" && firstNumber!="-")
               {
                   double result = 0;
                   double firstD = Convert.ToDouble(firstNumber);
                   double secondD = Convert.ToDouble(secondNumber);
                   switch (operation)
                   {
                       case "+":
                           result = firstD + secondD;
                           break;
                       case "-":
                           result = firstD - secondD;
                           break;
                       case "*":
                           result = firstD * secondD;
                           break;
                       case "/":
                                if(secondD == 0)
                                {
                                    Kernel.ShowMessage("no... just no", "Calculator", MsgType.Info);
                                    result = 0;
                                    return;
                                }
                           result = firstD / secondD;
                           break;
                   }
                   secondNumber = "";
                   isModifyingSecondField = false;
                   firstNumber = result.ToString();
                   operation = null;
               }
           }
       }catch(Exception ex)
       {
                Kernel.ShowMessage(ex.Message, "Calculator",MsgType.Error);
           Close();
       }
        }
    }
}
