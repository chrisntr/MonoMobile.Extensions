namespace MonoMobile.Extensions
{
    public interface IDevice
    {
        string Name { get; }
        string MonoMobileVersion { get; }
        string Platform { get; }
        string UUID { get; }
        string Version { get; }
    }
}