using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TransactionsApps.Models;
using TransactionsApps.Services;

namespace TransactionsApps.Controllers
{
    [AutoValidateAntiforgeryToken] // optionnel : applique l’antiforgery sur tous les POST
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var items = await _transactionService.GetAllTransactionsAsync();
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> AddOrEdit(int id = 0, CancellationToken ct = default)
        {
            if (id == 0) return View(new Transaction());

            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction is null) return NotFound();

            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // utile même si AutoValidateAntiforgeryToken est présent
        public async Task<IActionResult> AddOrEdit(
            // Overposting protection (choisis un ViewModel si besoin)
            [Bind("TransactionId,AccountNumber,BankName,SWIFTCode,Amount,DateCreation")]
            Transaction transaction,
            CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(transaction);

            try
            {
                await _transactionService.AddOrUpdateTransactionAsync(transaction);
                TempData["Success"] = transaction.TransactionId == 0
                    ? "Transaction créée avec succès."
                    : "Transaction mise à jour avec succès.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Transaction introuvable (Id: {Id})", transaction.TransactionId);
                TempData["Error"] = "La transaction demandée est introuvable.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l’enregistrement de la transaction");
                ModelState.AddModelError(string.Empty, "Une erreur est survenue. Réessayez plus tard.");
                return View(transaction);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct = default)
        {
            try
            {
                await _transactionService.DeleteTransactionAsync(id);
                TempData["Success"] = "Transaction supprimée.";
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "La transaction à supprimer est introuvable.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression (Id: {Id})", id);
                TempData["Error"] = "Une erreur est survenue pendant la suppression.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}


/*
using Microsoft.AspN
    etCore.Mvc;
using TransactionsApps.Models;
using TransactionsApps.Services;

namespace TransactionsApps.Controllers
{
    public class TransactionController(ITransactionService transactionService) 
        : Controller
    {
        private readonly ITransactionService _transactionService = transactionService;

        public async Task<IActionResult> Index()
        {
            return View(await _transactionService.GetAllTransactionsAsync());
        }

        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Transaction());

            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction == null)
                return NotFound();

            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                await _transactionService.AddOrUpdateTransactionAsync(transaction);
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _transactionService.DeleteTransactionAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

}
*/