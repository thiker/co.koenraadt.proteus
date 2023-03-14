using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.co.koenraadt.proteus.Runtime.Repositories
{
    public partial class Repository
    {
        private readonly Dictionary<string, PTViewer> _viewers = new();
        
        public PTViewer CreatePTViewer()
        {
            PTViewer viewer = new("test");
            _viewers[viewer.Id] = viewer;
            return viewer;
        }
    }
}
