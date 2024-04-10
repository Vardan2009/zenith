using Cosmos.System.Graphics;
using zenithos.Controls;

namespace zenithos.Windows
{
    internal class UITest : Window
    {
        public InputField field1;
      
       
        public UITest() : base(100, 100, 200, 100, "UI Test", Kernel.defFont, true)
        {
            field1 = new(20, 20, 100, font, 5);
          
            controls.Add(field1);
        }
        public override void Update(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            base.Update(canv, mX, mY, mD, dmX, dmY);
            if(resizing)
            {
                field1.width = w - 40;
            }
            if(field1.submittedOnce)
            {
                Kernel.ThrowError("Your Input is: " + field1.Value, "Test");
            }
            
        }
    }
}
