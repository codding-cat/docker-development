namespace Authentication.Models;
public enum Roles
{
    NoAuthorized,
    Base = 100,
    Application = 110,
    Support = 200,
    Administrator = 300,
    SuperAdministrator = 400
}