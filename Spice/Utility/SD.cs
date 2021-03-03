using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Utility
{
    public static class SD
    {
        public static string DefaultImage { get; } = @"\images\no_Poster.png";
        public static string ImageFolder { get; } = @"images";
        //Roles
        public const string ManagerUser = @"Manager";
        public const string KitchenUser = @"Kitchen";
        public const string FrontDeskUser = @"FrontDesk";
        public const string Customer = @"Customer";
        public static string[] RoleNames = { ManagerUser, KitchenUser, FrontDeskUser, Customer };
        public const string CartCount = "CartCount";
        public const string CartCoupon = "CartCoupon";

        public const string CouponApplied = "CouponApplied";
        public const string CouponRemoved = "CouponRemoved";

        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static double DiscountedPrice(Coupon coupon, double originalOrderTotal)
        {
            if (coupon == null)
            {
                return originalOrderTotal;
            }
            else
            {
                if (coupon.MinimumAmount > originalOrderTotal)
                {
                    return originalOrderTotal;
                }
                else
                {
                    switch (coupon.CouponType)
                    {
                        //10% off with 100
                        case CouponType.Percent:
                            return Math.Round(originalOrderTotal - (originalOrderTotal * coupon.Discount / 100), 2);

                        case CouponType.Dollar:
                            return Math.Round(originalOrderTotal - (originalOrderTotal * coupon.Discount / 100), 2);

                        //₹ 10 off with 100
                        case CouponType.Rupee:
                            return Math.Round(originalOrderTotal - coupon.Discount, 2);
                        default:
                            return originalOrderTotal;
                    }

                }
            }
        }
    }
    public enum CouponStatus
    {
        Idle,Applied,Removed
    }
}
