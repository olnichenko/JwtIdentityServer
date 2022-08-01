namespace JwtIdentityServer.Validators
{
    public interface IValidator<T> where T : class
    {
        bool Validate(T entity);
    }
}
