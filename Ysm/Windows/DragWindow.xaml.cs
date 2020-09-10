using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ysm.Core;
using Ysm.Models;

namespace Ysm.Windows
{
    public partial class DragWindow 
    {
        public ObservableCollection<Node> Nodes { get; set; }
        
        public DragWindow(List<Node> nodes)
        {
            InitializeComponent();

            Nodes = new ObservableCollection<Node>();
            
            Nodes.AddRange(nodes);

            DataContext = this;
        }
    }
}
