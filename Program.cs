namespace GATT_Client_Win11;

class Program
{
    static void Main(string[] args)
    {
        AdvertisementScanner.StartScanning();
        while (true) {}
    }
}
