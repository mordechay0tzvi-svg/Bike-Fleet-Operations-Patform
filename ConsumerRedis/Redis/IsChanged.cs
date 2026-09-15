using Models;
namespace Redis;
public static class StatusChecker
{
    public static bool IsStatusChanged(StationStatus previous, StationStatus current)
    {
        if (current.NumVehiclesAvailable != previous.NumVehiclesAvailable)
        {
            return true;
        }

        if (current.NumDocksAvailable != previous.NumDocksAvailable)
        {
            return true;
        }

        if (current.IsRenting != previous.IsRenting)
        {
            return true;
        }

        if (current.IsReturning != previous.IsReturning)
        {
            return true;
        }

        return false;
    }
}
