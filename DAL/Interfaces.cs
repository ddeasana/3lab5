
namespace DAL
{
    public interface IProvider<T> where T : class
    {
        public List<T> Load();
        public void Save(List<T> listToSave);
    }
    
    interface IStudy
    {
        public string Study();
    }
    interface IManage
    {
        public string Manage();
    }
    interface ICook
    {
        public string Cook();
    }
    interface IPLayChess
    {
        public string PlayChess();
    }
}
