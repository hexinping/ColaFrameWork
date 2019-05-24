//# FancyScrollView

//A scrollview component that can implement highly flexible animation.
//It also supports infinite scrolling.
//This library is free and opensource on GitHub.More info, supports and sourcecodes, see[https://github.com/setchi/FancyScrollView](https://github.com/setchi/FancyScrollView)

//## License
//MIT

using System.Linq;
using UnityEngine;

namespace FancyScrollView
{
    public class Example02Scene : MonoBehaviour
    {
        [SerializeField]
        Example02ScrollView scrollView;

        void Start()
        {
            var cellData = Enumerable.Range(0, 20)
                .Select(i => new Example02CellDto { Message = "Cell " + i })
                .ToList();

            scrollView.UpdateData(cellData);
        }
    }
}
