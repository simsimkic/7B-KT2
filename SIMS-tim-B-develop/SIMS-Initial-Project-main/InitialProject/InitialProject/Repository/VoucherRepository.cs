using InitialProject.Model;
using InitialProject.Resources.Observer;
using InitialProject.Serializer;
using System.Collections.Generic;
using System.Linq;

namespace InitialProject.Repository
{
    public class VoucherRepository : ISubject
    {
        private const string _filePath = "../../../Resources/Data/vouchers.csv";

        private readonly Serializer<Voucher> _serializer;
        private readonly List<IObserver> _observers;

        private List<Voucher> _vouchers;

        public VoucherRepository()
        {
            _observers = new List<IObserver>();
            _serializer = new Serializer<Voucher>();
            _vouchers = _serializer.FromCSV(_filePath);
        }

        public int NextId()
        {
            _vouchers = _serializer.FromCSV(_filePath);
            if (_vouchers.Count < 1)
            {
                return 1;
            }
            return _vouchers.Max(c => c.Id) + 1;
        }

        public Voucher Save(Voucher voucher)
        {
            voucher.Id = NextId();
            _vouchers = _serializer.FromCSV(_filePath);
            _vouchers.Add(voucher);
            _serializer.ToCSV(_filePath, _vouchers);
            NotifyObservers();

            return voucher;
        }
        public Voucher Update(Voucher voucher)
        {
            _vouchers = _serializer.FromCSV(_filePath);
            Voucher current = _vouchers.Find(c => c.Id == voucher.Id);
            int index = _vouchers.IndexOf(current);
            _vouchers.Remove(current);
            _vouchers.Insert(index, voucher);       // keep ascending order of ids in file 
            _serializer.ToCSV(_filePath, _vouchers);
            NotifyObservers();
            return voucher;
        }

        public Voucher GetById(int id)
        {
            _vouchers = _serializer.FromCSV(_filePath);
            return _vouchers.Find(c => c.Id == id);
        }

        public List<Voucher> GetAll()
        {
            return _vouchers;
        }

        public void Subscribe(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Unsubscribe(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            foreach (var observer in _observers)
            {
                observer.Update();
            }
        }
    }
}
