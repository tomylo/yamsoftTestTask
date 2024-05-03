namespace DBLogic.Contracts
{
    public interface IRepositoryWrapper
    {

        #region  Repository Interfaces 
        IRefreshTokenRepository RefreshTokenRepository { get; }
       
        #endregion


    }
}
