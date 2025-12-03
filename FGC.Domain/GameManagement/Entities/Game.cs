using FGC.Domain.Common.Entities;
using FGC.Domain.GameManagement.Enums;
using FGC.Domain.GameManagement.Events;

namespace FGC.Domain.GameManagement.Entities
{
    public class Game : AggregateRoot
    {
        #region [Properties]

        public string Title { get; private set; }

        public string Description { get; private set; }

        public decimal Price { get; private set; }

        public GameCategory Category { get; private set; }

        public string Developer { get; private set; }

        public string Publisher { get; private set; }

        public DateTime ReleaseDate { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime? UpdatedAt { get; private set; }

        public bool IsActive { get; private set; }

        public double Rating { get; private set; }

        public int TotalSales { get; private set; }

        #endregion

        #region [Constructor]

        private Game() : base() { }

        private Game(string title, string description, decimal price, GameCategory category, string developer, string publisher, DateTime releaseDate) : base()
        {
            Title = ValidateTitle(title);
            Description = ValidateDescription(description);
            Price = ValidatePrice(price);
            Category = category;
            Developer = ValidateDeveloper(developer);
            Publisher = ValidatePublisher(publisher);
            ReleaseDate = releaseDate;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            Rating = 0;
            TotalSales = 0;
        }

        #endregion

        #region [Factory Methods]

        public static Game Create(string title, string description, decimal price, GameCategory category, string developer, string publisher, DateTime releaseDate)
        {
            var game = new Game(title, description, price, category, developer, publisher, releaseDate);

            game.AddDomainEvent(new GameCreatedEvent(
                game.Id,
                game.Title,
                game.Price,
                game.Category.ToString(),
                game.CreatedAt
            ));

            return game;
        }

        #endregion

        #region [Business Methods]

        public void UpdatePrice(decimal newPrice)
        {
            if (!IsActive)
                throw new InvalidOperationException("Jogo inativo não pode ter preço alterado");

            var oldPrice = Price;
            Price = ValidatePrice(newPrice);
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new GamePriceUpdatedEvent(Id, Title, oldPrice, newPrice, DateTime.UtcNow));
        }

        public void UpdateDetails(string title, string description)
        {
            if (!IsActive)
                throw new InvalidOperationException("Jogo inativo não pode ser alterado");

            Title = ValidateTitle(title);
            Description = ValidateDescription(description);
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException("Jogo já está inativo");

            IsActive = false;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new GameDeactivatedEvent(Id, Title, DateTime.UtcNow));
        }

        public void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException("Jogo já está ativo");

            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RegisterSale()
        {
            TotalSales++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateRating(double newRating)
        {
            if (newRating < 0 || newRating > 5)
                throw new ArgumentException("Rating deve estar entre 0 e 5");

            Rating = Math.Round(newRating, 1);
            UpdatedAt = DateTime.UtcNow;
        }

        #endregion

        #region [Validations]

        private static string ValidateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Título é obrigatório", nameof(title));

            if (title.Length < 2)
                throw new ArgumentException("Título deve ter pelo menos 2 caracteres", nameof(title));

            if (title.Length > 200)
                throw new ArgumentException("Título deve ter no máximo 200 caracteres", nameof(title));

            return title.Trim();
        }

        private static string ValidateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Descrição é obrigatória", nameof(description));

            if (description.Length > 2000)
                throw new ArgumentException("Descrição deve ter no máximo 2000 caracteres", nameof(description));

            return description.Trim();
        }

        private static decimal ValidatePrice(decimal price)
        {
            if (price < 0)
                throw new ArgumentException("Preço não pode ser negativo", nameof(price));

            if (price > 999999.99m)
                throw new ArgumentException("Preço excede o valor máximo permitido", nameof(price));

            return Math.Round(price, 2);
        }

        private static string ValidateDeveloper(string developer)
        {
            if (string.IsNullOrWhiteSpace(developer))
                throw new ArgumentException("Desenvolvedor é obrigatório", nameof(developer));

            return developer.Trim();
        }

        private static string ValidatePublisher(string publisher)
        {
            if (string.IsNullOrWhiteSpace(publisher))
                throw new ArgumentException("Publicador é obrigatório", nameof(publisher));

            return publisher.Trim();
        }

        #endregion
    }
}
