using System.Text;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
namespace GATT_Client_Win11;

class GetBLEData
{
    private static void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
    {
        // An Indicate or Notify reported that the value has changed.
        var reader = DataReader.FromBuffer(args.CharacteristicValue);
        byte[] input = new byte[reader.UnconsumedBufferLength];
        reader.ReadBytes(input);
        string str = Encoding.ASCII.GetString(input);
        Console.WriteLine($"result: {str}");
    }
    public static async void GetData(BluetoothLEDevice bluetoothLeDevice)
    {
        Guid serviceUuid = new Guid("00055000-0000-0000-0001-000000000000");
        Guid characteristicUuid = new Guid("00055000-0000-0000-0001-000000000002");

        GattDeviceServicesResult serviceResult = await bluetoothLeDevice.GetGattServicesAsync();
        
        switch (serviceResult.Status)
        {
            case GattCommunicationStatus.Success: 
                Console.WriteLine("Communicator: Success");
                break;
            case GattCommunicationStatus.AccessDenied:
                Console.WriteLine("Communicator: AccessDenied");
                break;
            case GattCommunicationStatus.Unreachable:
                Console.WriteLine("Communicator: Unreachable");
                break;
            default:
            case GattCommunicationStatus.ProtocolError:
                Console.WriteLine("Communicator: ProtocolError");
                break;
        }
        if (serviceResult.Status != GattCommunicationStatus.Success)
        {
            Console.WriteLine("Communicator: cannot get GATT services");
            GetData(bluetoothLeDevice);
            return;
        }

        var services = serviceResult.Services;
        foreach (var service in services)
        {
            if (service.Uuid != serviceUuid) { continue; }

            GattCharacteristicsResult characterResult = await service.GetCharacteristicsAsync();
            if (characterResult.Status != GattCommunicationStatus.Success)
            {
                Console.WriteLine("Communicator: cannot get GATT characteristics");
                continue;
            }

            var characteristics = characterResult.Characteristics;
            foreach (var characteristic in characteristics)
            {
                if (characteristic.Uuid != characteristicUuid) { continue; }
                // Console.WriteLine($"characteristic uuid {characteristic.Uuid}");

                GattCharacteristicProperties properties = characteristic.CharacteristicProperties;

                if (properties.HasFlag(GattCharacteristicProperties.Read))
                {
                    GattReadResult result = await characteristic.ReadValueAsync();
                    if (result.Status == GattCommunicationStatus.Success)
                    {
                        var reader = DataReader.FromBuffer(result.Value);
                        byte[] input = new byte[reader.UnconsumedBufferLength];
                        reader.ReadBytes(input);
                        string str = Encoding.ASCII.GetString(input);
                        Console.WriteLine($"result: {str}");
                    }
                }

                if (properties.HasFlag(GattCharacteristicProperties.Notify))
                {
                    GattCommunicationStatus status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                        GattClientCharacteristicConfigurationDescriptorValue.Notify);
                    if (status == GattCommunicationStatus.Success)
                    {
                        characteristic.ValueChanged += Characteristic_ValueChanged;
                    }
                }
            }
        }
    }
}