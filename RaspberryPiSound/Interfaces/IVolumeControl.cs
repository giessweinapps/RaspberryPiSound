namespace RaspberryPiSound.Interfaces
{
    public interface IVolumeControl
    {
        int Volume { get; }
        void ChangeVolume(string direction);
    }
}