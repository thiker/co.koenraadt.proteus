using Packages.co.koenraadt.proteus.Runtime.ViewModels;
namespace Packages.co.koenraadt.proteus.Runtime.Repositories
{

    /// <summary>
    /// The repository holding all data of Proteus.
    /// </summary>
    public class Repository
    {
        private static Repository _instance = null;

        /// <summary>
        /// The singleton instance of the repository.
        /// </summary>
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

        /// <summary>
        /// Singleton instance of the repository part that holds all general Proteus data.
        /// </summary>
        public ProteusRepository Proteus {
            get {
                return ProteusRepository.Instance;
            }
        }

        /// <summary>
        /// Singleton instance of the repository part that holds all models related data.
        /// </summary>
        public ModelsRepository Models {
            get {
                return ModelsRepository.Instance;
            }
        }

        /// <summary>
        /// Singleton instance of the repository part that holds all states related data.
        /// </summary>
        public StatesRepository States
        {
            get
            {
                return StatesRepository.Instance;
            }
        }

        /// <summary>
        /// Singleton instance of the repository part that holds all viewer related data.
        /// </summary>
        public ViewersRepository Viewers
        {
            get
            {
                return ViewersRepository.Instance;
            }
        }
    }
}