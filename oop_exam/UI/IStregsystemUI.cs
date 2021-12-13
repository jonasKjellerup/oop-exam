using oop_exam.Models;

namespace oop_exam.UI
{
	public delegate void StregsystemEvent(string input);
	
	public interface IStregsystemUI
	{
		void DisplayUserNotFound(string username);
		void DisplayProductNotFound(uint product);
		void DisplayUserInfo(User user);
		void DisplayTooManyArgumentsError(string command);
		void DisplayAdminCommandNotFoundMessage(string adminCommand);
		void DisplayUserBuysProduct(BuyTransaction transaction);
		void DisplayUserBuysProduct(uint count, BuyTransaction transaction);
		void Close();
		void DisplayInsufficientCash(User user, Product product, uint count=1);
		void DisplayGeneralError(string errorString);
		void Start();
		event StregsystemEvent CommandEntered;
	}
}