using FGC.Domain.UserManagement.ValueObjects;
using FluentAssertions;

namespace FGC.Domain.Tests
{
    public class EmailTests
    {
        #region [Cenários de SUCESSO - Devem PASSAR]

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Success")]
        // Valida que um email com formato básico válido é criado corretamente
        public void Constructor_WithValidSimpleEmail_ShouldCreateSuccessfully()
        {
            //Arrange (Preparar)
            var validEmail = "user@fgc.com";

            //Act (Agir) 
            var email = new Email(validEmail);

            //Assert (Verificar)
            email.Value.Should().Be("user@fgc.com");
            email.Should().NotBeNull();
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Success")]
        // Valida que emails com subdomínios são aceitos
        public void Constructor_WithValidEmailWithSubdomain_ShouldCreateSuccessfully()
        {
            //Arrange (Preparar)
            var validEmail = "admin@mail.fgc.com";

            //Act (Agir) 
            var email = new Email(validEmail);

            //Assert (Verificar)
            email.Value.Should().Be("admin@mail.fgc.com");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Success")]
        // Valida que emails com números são aceitos
        public void Constructor_WithValidEmailWithNumbers_ShouldCreateSuccessfully()
        {
            //Arrange (Preparar)
            var validEmail = "user123@fgc2024.com";

            //Act (Agir) 
            var email = new Email(validEmail);

            //Assert (Verificar)
            email.Value.Should().Be("user123@fgc2024.com");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Success")]
        // Valida que emails em maiúscula são normalizados para minúscula
        public void Constructor_WithUppercaseEmail_ShouldNormalizeToLowercase()
        {
            //Arrange (Preparar)
            var uppercaseEmail = "USER@FGC.COM";

            //Act (Agir) 
            var email = new Email(uppercaseEmail);

            //Assert (Verificar)
            email.Value.Should().Be("user@fgc.com");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Success")]
        // Valida que espaços no início e fim são removidos automaticamente
        public void Constructor_WithEmailWithSpaces_ShouldTrimSpaces()
        {
            //Arrange (Preparar)
            var emailWithSpaces = "  user@fgc.com  ";

            //Act (Agir) 
            var email = new Email(emailWithSpaces);

            //Assert (Verificar)
            email.Value.Should().Be("user@fgc.com");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Success")]
        // Valida que emails longos (até 254 chars) são aceitos
        public void Constructor_WithValidLongEmail_ShouldCreateSuccessfully()
        {
            //Arrange (Preparar)
            var longEmail = new string('a', 240) + "@fgc.com";

            //Act (Agir) 
            var email = new Email(longEmail);

            //Assert (Verificar)
            email.Value.Should().Be(longEmail.ToLowerInvariant());
        }

        #endregion

        #region [Cenários de FALHA - Devem LANÇAR EXCEÇÃO]

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que email null lança exceção com mensagem específica
        public void Constructor_WithNullEmail_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            string nullEmail = null;

            //Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(nullEmail));
            exception.Message.Should().Be("Email não pode ser vazio ou nulo");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que string vazia lança exceção apropriada
        public void Constructor_WithEmptyEmail_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            var emptyEmail = "";

            //Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(emptyEmail));
            exception.Message.Should().Be("Email não pode ser vazio ou nulo");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que apenas espaços em branco são rejeitados
        public void Constructor_WithWhitespaceEmail_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            var whitespaceEmail = "   ";

            //Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(whitespaceEmail));
            exception.Message.Should().Be("Email não pode ser vazio ou nulo");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que email sem @ é rejeitado como formato inválido
        public void Constructor_WithEmailWithoutAtSymbol_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            var invalidEmail = "userfgc.com";

            //Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
            exception.Message.Should().Be("Formato de email inválido");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que email sem domínio após @ é rejeitado
        public void Constructor_WithEmailWithoutDomain_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            var invalidEmail = "user@";

            //Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
            exception.Message.Should().Be("Formato de email inválido");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que email sem parte local antes do @ é rejeitado
        public void Constructor_WithEmailWithoutLocalPart_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            var invalidEmail = "@fgc.com";

            //Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
            exception.Message.Should().Be("Formato de email inválido");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que emails muito longos (>254 chars) são rejeitados
        public void Constructor_WithTooLongEmail_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            var tooLongEmail = new string('a', 250) + "@fgc.com";

            //Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(tooLongEmail));
            exception.Message.Should().Be("Email muito longo. Máximo de 254 caracteres");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que múltiplos símbolos @ são rejeitados
        public void Constructor_WithMultipleAtSymbols_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            var invalidEmail = "user@@fgc.com";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
            exception.Message.Should().Be("Formato de email inválido");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que espaços dentro do email são rejeitados
        public void Constructor_WithEmailWithSpacesInside_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            var invalidEmail = "user name@fgc.com";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
            exception.Message.Should().Be("Formato de email inválido");
        }

        #endregion

        #region [Testes de COMPORTAMENTO]

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Behavior")]
        // Valida que Value Objects com mesmo valor são considerados iguais
        public void TwoEmailsWithSameValue_ShouldBeEqual()
        {
            //Arrange (Preparar)
            var email1 = new Email("user@fgc.com");
            var email2 = new Email("user@fgc.com");

            //Act & Assert
            email1.Should().Be(email2);
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Behavior")]
        // Valida que Value Objects com valores diferentes não são iguais
        public void TwoEmailsWithDifferentValues_ShouldNotBeEqual()
        {
            //Arrange (Preparar)
            var email1 = new Email("user1@fgc.com");
            var email2 = new Email("user2@fgc.com");

            //Act & Assert
            email1.Should().NotBe(email2);
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Behavior")]
        // Valida que ToString() retorna o valor do email corretamente
        public void ToString_ShouldReturnEmailValue()
        {
            //Arrange (Preparar)
            var emailValue = "user@fgc.com";
            var email = new Email(emailValue);

            //Act (Agir)
            var result = email.ToString();

            //Assert (Verificar)
            result.Should().Be(emailValue);
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Behavior")]
        // Valida que conversão implícita para string funciona corretamente
        public void ImplicitConversion_ShouldReturnEmailValue()
        {
            //Arrange (Preparar)
            var email = new Email("user@fgc.com");

            //Act (Agir)
            string emailAsString = email;

            //Assert (Verificar)
            emailAsString.Should().Be("user@fgc.com");
        }

        #endregion
    }
}