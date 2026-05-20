using System;
using System.Collections.Generic;

namespace FareEngineAssessment
{
    
    public enum TripStatus
    {
        Pending,
        Paid,
        Failed
    }

   
    public class InvalidTripException : Exception
    {
        public InvalidTripException(string message) : base(message)
        {
        }
    }

    public record Passenger(string PassengerId, string Name);

    public interface IPromotion
    {
        decimal ApplyDiscount(decimal currentFare);
    }

 
    public interface IPaymentService
    {
        bool ProcessPayment(string passengerId, decimal amount);
    }

    public abstract class Vehicle
    {
        public string LicensePlate { get; private set; }
        public decimal BaseFare { get; private set; }

        protected Vehicle(string licensePlate, decimal baseFare)
        {
            if (string.IsNullOrWhiteSpace(licensePlate))
                throw new ArgumentException("License plate cannot be empty.");

            if (baseFare < 0)
                throw new ArgumentException("Base fare cannot be negative.");

            LicensePlate = licensePlate;
            BaseFare = baseFare;
        }

        public abstract decimal PerKmRate { get; }
        public abstract decimal PerMinuteRate { get; }

        public virtual decimal CalculateFare(decimal distanceKms, int durationMinutes)
        {
            decimal fare = BaseFare + (distanceKms * PerKmRate) +(durationMinutes * PerMinuteRate);

            return fare;
        }
    }

   
    public class StandardCar : Vehicle
    {
        public StandardCar(string licensePlate, decimal baseFare)
            : base(licensePlate, baseFare)
        {
        }

        public override decimal PerKmRate => 10m;
        public override decimal PerMinuteRate => 2m;
    }


    public class LuxurySedan : Vehicle
    {
        public decimal LuxuryTax { get; private set; }

        public LuxurySedan(string licensePlate, decimal baseFare, decimal luxuryTax)
            : base(licensePlate, baseFare)
        {
            LuxuryTax = luxuryTax;
        }

        public override decimal PerKmRate => 20m;
        public override decimal PerMinuteRate => 5m;

        public override decimal CalculateFare(decimal distanceKms, int durationMinutes)
        {
            decimal fare = base.CalculateFare(distanceKms, durationMinutes);

            fare += LuxuryTax;

            return fare;
        }
    }


    public class PercentageDiscount : IPromotion
    {
        public decimal Percentage { get; private set; }

        public PercentageDiscount(decimal percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentException("Invalid percentage.");

            Percentage = percentage;
        }

        public decimal ApplyDiscount(decimal currentFare)
        {
            decimal discount = currentFare * (Percentage / 100m);

            return currentFare - discount;
        }
    }

   
    public class FlatDiscount : IPromotion
    {
        public decimal DiscountAmount { get; private set; }

        public FlatDiscount(decimal discountAmount)
        {
            if (discountAmount < 0)
                throw new ArgumentException("Discount cannot be negative.");

            DiscountAmount = discountAmount;
        }

        public decimal ApplyDiscount(decimal currentFare)
        {
            return currentFare - DiscountAmount;
        }
    }

    // payment services implements

    public class CreditCardPaymentService : IPaymentService
    {
        public bool ProcessPayment(string passengerId, decimal amount)
        {
            Console.WriteLine($"Processing payment for Passenger ID: {passengerId}");
            Console.WriteLine($"Amount: {amount}");

            return true;
        }
    }


    public class Trip
    {
        public Vehicle Vehicle { get; private set; }
        public Passenger Passenger { get; private set; }

        public decimal DistanceKms { get; private set; }
        public int DurationMinutes { get; private set; }

        public IPromotion? Promotion { get; private set; }

        public TripStatus Status { get; private set; }

        public Trip(
            Vehicle vehicle,
            Passenger passenger,
            decimal distanceKms,
            int durationMinutes,
            IPromotion? promotion = null)
        {
            Vehicle = vehicle ?? throw new InvalidTripException("Vehicle cannot be null.");

            Passenger = passenger ?? throw new InvalidTripException("Passenger cannot be null.");

            if (distanceKms < 0)
                throw new InvalidTripException("Distance cannot be negative.");

            if (durationMinutes <= 0)
                throw new InvalidTripException("Duration must be greater than zero.");

            DistanceKms = distanceKms;
            DurationMinutes = durationMinutes;
            Promotion = promotion;

            Status = TripStatus.Pending;
        }

       
        public decimal CalculateFinalFare()
        {
            if (Vehicle == null)
                throw new InvalidOperationException("No vehicle assigned.");

            decimal fare = Vehicle.CalculateFare(DistanceKms, DurationMinutes);

           
            if (Promotion != null)
            {
                fare = Promotion.ApplyDiscount(fare);
            }

            
            if (fare < Vehicle.BaseFare)
            {
                fare = Vehicle.BaseFare;
            }

            return fare;
        }

        
        public void CompleteTrip(IPaymentService paymentService)
        {
            decimal finalFare = CalculateFinalFare();

            bool paymentSuccess =
                paymentService.ProcessPayment(
                    Passenger.PassengerId,
                    finalFare
                );

            Status = paymentSuccess
                ? TripStatus.Paid
                : TripStatus.Failed;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            
               
                Passenger passenger =
                    new Passenger("DTT-154", "Nayan Chandra");

               
                Vehicle standardCar =
                    new StandardCar("DKIEK-5484544", 60m);

                Vehicle luxurySedan =
                    new LuxurySedan("KJDLJI-9884651", 110m, 30m);

              
                IPromotion percentagePromo =
                    new PercentageDiscount(10);

                IPromotion flatPromo =
                    new FlatDiscount(80m);

                IPaymentService paymentService =
                    new CreditCardPaymentService();

                
                Trip trip1 = new Trip(
                    standardCar,
                    passenger,
                    10,
                    20,
                    percentagePromo
                );

                decimal fare1 = trip1.CalculateFinalFare();

                Console.WriteLine("STANDARD CAR.......");
                Console.WriteLine($"Final Fare: {fare1}");

                trip1.CompleteTrip(paymentService);

                Console.WriteLine($"Trip Status: {trip1.Status}");
                Console.WriteLine();

             
                Trip trip2 = new Trip(
                    luxurySedan,
                    passenger,
                    15,
                    30,
                    flatPromo
                );

                decimal fare2 = trip2.CalculateFinalFare();

                Console.WriteLine("LUXURY SEDAN.......");
                Console.WriteLine($"Final Fare: {fare2}");

                trip2.CompleteTrip(paymentService);

                Console.WriteLine($"Trip Status: {trip2.Status}");
                Console.WriteLine();

                
            }
            
        }
}
