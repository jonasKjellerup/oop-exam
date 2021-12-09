using oop_exam.Models;

namespace oop_exam.UI
{
	public delegate void StregsystemEvent();
	
	public interface IStregsystemUI
	{
		void DisplayUserNotFound(string username);
		void DisplayProductNotFound(string product);
		void DisplayUserInfo(User user);
		void DisplayTooManyArgumentsError(string command);
		void DisplayAdminCommandNotFoundMessage(string adminCommand);
		void DisplayUserBuysProduct(BuyTransaction transaction);
		void DisplayUserBuysProduct(int count, BuyTransaction transaction);
		void Close();
		void DisplayInsufficientCash(User user, Product product);
		void DisplayGeneralError(string errorString);
		void Start();
		event StregsystemEvent CommandEntered;
	}
}