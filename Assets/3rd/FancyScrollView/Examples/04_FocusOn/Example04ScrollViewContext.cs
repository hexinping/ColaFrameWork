//# FancyScrollView

//A scrollview component that can implement highly flexible animation.
//It also supports infinite scrolling.
//This library is free and opensource on GitHub.More info, supports and sourcecodes, see[https://github.com/setchi/FancyScrollView](https://github.com/setchi/FancyScrollView)

//## License
//MIT

using System;

namespace FancyScrollView
{
    public class Example04ScrollViewContext
    {
        int selectedIndex = -1;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (value == selectedIndex)
                {
                    return;
                }

                selectedIndex = value;

                if (OnSelectedIndexChanged != null)
                {
                    OnSelectedIndexChanged(selectedIndex);
                }
            }
        }

        public Action<Example04ScrollViewCell> OnPressedCell;
        public Action<int> OnSelectedIndexChanged;
    }
}
