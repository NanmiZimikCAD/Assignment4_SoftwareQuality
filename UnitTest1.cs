using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace InsuranceTests
{
    public class Tests
    {
        private IWebDriver driver;
        bool shouldsroll=true;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("http://localhost/prog8173_A04/");
            Thread.Sleep(1500);
            driver.FindElement(By.CssSelector("a.btn.btn-secondary.btn-lg.btn-block")).Click();
            Thread.Sleep(1500);
        }

        [Test]
        public void InsuranceQuote01_ValidNoAccident()
        {
            FillForm("Nanmi", "Zimik", "123 Main St", "Milton", "ON", "L9A 0B1", "123-123-1234", "test1@mail.com", "25", "3", "0");
            SubmitForm(true);
            AssertQuoteIsPopulated();
        }

        [Test]
        public void InsuranceQuote02_ValidSomeAccidents()
        {
            FillForm("Nanmi", "Zimik", "123 Main St", "Milton", "ON", "L9A 0B1", "123-123-1234", "test2@mail.com", "25", "3", "2");
            SubmitForm(true);
            AssertQuoteIsPopulated();
        }

        [Test]
        public void InsuranceQuote03_TooManyAccidents()
        {
            FillForm("Nanmi", "Zimik", "123 Main St", "Milton", "ON", "L9A 0B1", "123-123-1234", "test3@mail.com", "35", "10", "4");
            SubmitForm(true);
            string quote = driver.FindElement(By.Id("finalQuote")).GetAttribute("value");
            Assert.That(quote, Does.Contain("Too many accidents"), "Expected denial due to too many accidents.");
        }

        [Test]
        public void InsuranceQuote04_InvalidPhone()
        {
            FillForm("Nanmi", "Zimik", "123 Main St", "Milton", "ON", "L9A 0B1", "123123123", "test4@mail.com", "27", "3", "0");
            SubmitForm(false);
            AssertQuoteIsEmpty();
        }

        [Test]
        public void InsuranceQuote05_InvalidEmail()
        {
            FillForm("Nanmi", "Zimik", "123 Main St", "Milton", "ON", "L9A 0B1", "123-123-1234", "invalidemail.com", "28", "3", "0");
            SubmitForm(false);
            AssertQuoteIsEmpty();
        }

        [Test]
        public void InsuranceQuote06_InvalidPostalCode()
        {
            FillForm("Nanmi", "Zimik", "123 Main St", "Milton", "ON", "123456", "123-123-1234", "test6@mail.com", "35", "17", "1");
            SubmitForm(false);
            AssertQuoteIsEmpty();
        }

        [Test]
        public void InsuranceQuote07_MissingAge()
        {
            FillForm("Nanmi", "Zimik", "123 Main St", "Milton", "ON", "L9A 0B1", "123-123-1234", "test7@mail.com", "", "5", "0");
            SubmitForm(true);
            AssertQuoteIsEmpty();
        }

        [Test]
        public void InsuranceQuote08_MissingAccidents()
        {
            FillForm("Nanmi", "Zimik", "123 Main St", "Milton", "ON", "L9A 0B1", "123-123-1234", "test8@mail.com", "37", "8", "");
            SubmitForm(true);
            AssertQuoteIsEmpty();
        }

        [Test]
        public void InsuranceQuote09_MissingExperience()
        {
            FillForm("Nanmi", "Zimik", "123 Main St", "Milton", "ON", "L9A 0B1", "123-123-1234", "test9@mail.com", "45", "", "0");
            SubmitForm(true);
            AssertQuoteIsEmpty();
        }

        [Test]
        public void InsuranceQuote10_ZeroExperienceRate6000()
        {
            FillForm("Nanmi", "Zimik", "123 Main St", "Milton", "ON", "L9A 0B1", "123-123-1234", "test10@mail.com", "26", "0", "0");
            SubmitForm(true);
            AssertQuoteIsPopulated();
        }

        [Test]
        public void InsuranceQuote11_ExperienceBetween1And9()
        {
            FillForm("Nanmi", "Zimik", "123 Main St", "Milton", "ON", "L9A 0B1", "123-123-1234", "test11@mail.com", "29", "4", "1");
            SubmitForm(true);
            AssertQuoteIsPopulated();
        }

        [Test]
        public void InsuranceQuote12_ExperienceAbove9()
        {
            FillForm("Nanmi", "Zimik", "123 Main St", "Milton", "ON", "L9A 0B1", "123-123-1234", "test12@mail.com", "40", "15", "2");
            SubmitForm(true);
            AssertQuoteIsPopulated();
        }

        [Test]
        public void InsuranceQuote13_AgeReductionApplied()
        {
            FillForm("Nanmi", "Zimik", "123 Main St", "Milton", "ON", "L9A 0B1", "123-123-1234", "test13@mail.com", "32", "5", "0");
            SubmitForm(true);
            AssertQuoteIsPopulated();
        }

        [Test]
        public void InsuranceQuote14_ExperienceTooHighForAge()
        {
            FillForm("Nanmi", "Zimik", "123 Main St", "Milton", "ON", "L9A 0B1", "123-123-1234", "test14@mail.com", "40", "30", "1");
            SubmitForm(true);
            AssertQuoteIsPopulated();
        }

        [Test]
        public void InsuranceQuote15_ExperienceInvalidOverAgeLimit()
        {
            FillForm("Nanmi", "Zimik", "123 Main St", "Milton", "ON", "L9A 0B1", "123-123-1234", "test15@mail.com", "35", "25", "2");
            SubmitForm(true);
            string quote = driver.FindElement(By.Id("finalQuote")).GetAttribute("value");
            Assert.That(quote, Does.Contain("Driver Age / Experience Not Correct"), "Expected denial due to invalid experience.");
        }

        private void FillForm(string first, string last, string address, string city, string province, string postal, string phone, string email, string age, string exp, string acc)
        {
            driver.FindElement(By.Id("firstName")).Clear();
            driver.FindElement(By.Id("firstName")).SendKeys(first); Thread.Sleep(500);

            driver.FindElement(By.Id("lastName")).Clear();
            driver.FindElement(By.Id("lastName")).SendKeys(last); Thread.Sleep(500);

            driver.FindElement(By.Id("address")).Clear();
            driver.FindElement(By.Id("address")).SendKeys(address); Thread.Sleep(500);

            driver.FindElement(By.Id("city")).Clear();
            driver.FindElement(By.Id("city")).SendKeys(city); Thread.Sleep(500);

            new SelectElement(driver.FindElement(By.Id("province"))).SelectByValue(province); Thread.Sleep(500);

            driver.FindElement(By.Id("postalCode")).Clear();
            driver.FindElement(By.Id("postalCode")).SendKeys(postal); Thread.Sleep(500);

            driver.FindElement(By.Id("phone")).Clear();
            driver.FindElement(By.Id("phone")).SendKeys(phone); Thread.Sleep(500);

            driver.FindElement(By.Id("email")).Clear();
            driver.FindElement(By.Id("email")).SendKeys(email); Thread.Sleep(500);

            if (!string.IsNullOrWhiteSpace(age))
            {
                driver.FindElement(By.Id("age")).Clear();
                driver.FindElement(By.Id("age")).SendKeys(age); Thread.Sleep(500);
            }

            if (!string.IsNullOrWhiteSpace(exp))
            {
                driver.FindElement(By.Id("experience")).Clear();
                driver.FindElement(By.Id("experience")).SendKeys(exp); Thread.Sleep(500);
            }

            if (!string.IsNullOrWhiteSpace(acc))
            {
                driver.FindElement(By.Id("accidents")).Clear();
                driver.FindElement(By.Id("accidents")).SendKeys(acc); Thread.Sleep(500);
            }
        }

        private void SubmitForm(bool shouldScroll)
        {
            driver.FindElement(By.Id("btnSubmit")).Click();
	    thread.sleep(1000);
        }

        private void AssertQuoteIsPopulated()
        {
            string quote = driver.FindElement(By.Id("finalQuote")).GetAttribute("value");
            Assert.That(string.IsNullOrEmpty(quote), Is.False, "Expected quote to be populated, but it was empty.");
        }

        private void AssertQuoteIsEmpty()
        {
            string quote = driver.FindElement(By.Id("finalQuote")).GetAttribute("value");
            Assert.That(string.IsNullOrEmpty(quote), Is.True, "Expected quote to be empty, but it was populated.");
        }

        [TearDown]
        public void Teardown()
        {
            driver?.Quit();
            driver?.Dispose();
        }
    }
}
