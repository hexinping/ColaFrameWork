//# FancyScrollView

//A scrollview component that can implement highly flexible animation.
//It also supports infinite scrolling.
//This library is free and opensource on GitHub.More info, supports and sourcecodes, see[https://github.com/setchi/FancyScrollView](https://github.com/setchi/FancyScrollView)

//## License
//MIT

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FancyScrollView
{
    public class Example04Scene : MonoBehaviour
    {
        [SerializeField]
        Example04ScrollView scrollView;
        [SerializeField]
        Button prevCellButton;
        [SerializeField]
        Button nextCellButton;
        [SerializeField]
        Text selectedItemInfo;

        void Start()
        {
            prevCellButton.onClick.AddListener(scrollView.SelectPrevCell);
            nextCellButton.onClick.AddListener(scrollView.SelectNextCell);
            scrollView.OnSelectedIndexChanged(HandleSelectedIndexChanged);

            var cellData = Enumerable.Range(0, 20)
                .Select(i => new Example04CellDto { Message = "Cell " + i })
                .ToList();

            scrollView.UpdateData(cellData);
            scrollView.UpdateSelection(0);
        }

        void HandleSelectedIndexChanged(int index)
        {
            selectedItemInfo.text = String.Format("Selected item info: index {0}", index);
        }
    }
}
