using FGC.Domain.UserManagement.ValueObjects;
using FluentAssertions;

namespace FGC.Domain.Tests
{
    public class EmailTests
    {
        #region [Cen�rios de SUCESSO - Devem PASSAR]

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Success")]
        // Valida que um email com formato b�sico v�lido � criado corretamente
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
        // Valida que emails com subdom�nios s�o aceitos
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
        // Valida que emails com n�meros s�o aceitos
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
        // Valida que emails em mai�scula s�o normalizados para min�scula
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
        // Valida que espa�os no in�cio e fim s�o removidos automaticamente
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
        // Valida que emails longos (at� 254 chars) s�o aceitos
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

        #region [Cen�rios de FALHA - Devem LAN�AR EXCE��O]

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que email null lan�a exce��o com mensagem espec�fica
        public void Constructor_WithNullEmail_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            string nullEmail = null;

            //Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(nullEmail));
            exception.Message.Should().Be("Email n�o pode ser vazio ou nulo");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que string vazia lan�a exce��o apropriada
        public void Constructor_WithEmptyEmail_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            var emptyEmail = "";

            //Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(emptyEmail));
            exception.Message.Should().Be("Email n�o pode ser vazio ou nulo");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que apenas espa�os em branco s�o rejeitados
        public void Constructor_WithWhitespaceEmail_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            var whitespaceEmail = "   ";

            //Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(whitespaceEmail));
            exception.Message.Should().Be("Email n�o pode ser vazio ou nulo");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que email sem @ � rejeitado como formato inv�lido
        public void Constructor_WithEmailWithoutAtSymbol_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            var invalidEmail = "userfgc.com";

            //Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
            exception.Message.Should().Be("Formato de email inv�lido");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que email sem dom�nio ap�s @ � rejeitado
        public void Constructor_WithEmailWithoutDomain_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            var invalidEmail = "user@";

            //Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
            exception.Message.Should().Be("Formato de email inv�lido");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que email sem parte local antes do @ � rejeitado
        public void Constructor_WithEmailWithoutLocalPart_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            var invalidEmail = "@fgc.com";

            //Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
            exception.Message.Should().Be("Formato de email inv�lido");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que emails muito longos (>254 chars) s�o rejeitados
        public void Constructor_WithTooLongEmail_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            var tooLongEmail = new string('a', 250) + "@fgc.com";

            //Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(tooLongEmail));
            exception.Message.Should().Be("Email muito longo. M�ximo de 254 caracteres");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que m�ltiplos s�mbolos @ s�o rejeitados
        public void Constructor_WithMultipleAtSymbols_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            var invalidEmail = "user@@fgc.com";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
            exception.Message.Should().Be("Formato de email inv�lido");
        }

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Failure")]
        // Valida que espa�os dentro do email s�o rejeitados
        public void Constructor_WithEmailWithSpacesInside_ShouldThrowArgumentException()
        {
            //Arrange (Preparar)
            var invalidEmail = "user name@fgc.com";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
            exception.Message.Should().Be("Formato de email inv�lido");
        }

        #endregion

        #region [Testes de COMPORTAMENTO]

        [Fact]
        [Trait("Category", "TDD")]
        [Trait("Scenario", "Behavior")]
        // Valida que Value Objects com mesmo valor s�o considerados iguais
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
        // Valida que Value Objects com valores diferentes n�o s�o iguais
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
        // Valida que convers�o impl�cita para string funciona corretamente
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