using Packages.co.koenraadt.proteus.Runtime.ViewModels;
namespace Packages.co.koenraadt.proteus.Runtime.Repositories
{

    public class Repository
    {
        private static Repository _instance = null;
        public static Repository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Repository();
                }
                return _instance;
            }
        }

        public ProteusRepository Proteus {
            get {
                return ProteusRepository.Instance;
            }
        }

        public ModelsRepository Models {
            get {
                return ModelsRepository.Instance;
            }
        }
        public ViewersRepository Viewers
        {
            get
            {
                return ViewersRepository.Instance;
            }
        }
    }
}