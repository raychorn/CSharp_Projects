using System.Runtime.Intrinsics.X86;
using System.Text;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//Problem: Fruit calculator

//The solution needs to be written in C#. The project type, input and output is the developer’s choice.

//We have an application that takes as an input some fruits and their prices, promotions applicable and a basket. The output is the total price.

//Example 1:

//Input : Oranges – $10; Apples- $5 ; Promotions: No; Basket: Oranges - 5, Apples 1

//Output: Total price= 55

//Example 2:

//Input : Oranges – $10; Apples- $5 ; Promotions: Oranges – 0.5;  Basket: Oranges - 5, Apples 1

//Output: Total price= 30

namespace fruit_calc
{
    public static class Extensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict,TKey key)
        =>  dict.TryGetValue(key, out var value) ? value : default(TValue);
    }
    public class FruitCalc
    {
        public Dictionary<string, float> clean_the_basket(string[] baskets) {
            var cleanBasket = new Dictionary<string, float>();
            int i = 0;
            string key = "";
            float val = (float) -1.0;
            foreach (string basket_item in baskets) {
                string basket_val = Regex.Replace(basket_item, @"[^a-zA-Z0-9\.]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));
                if (basket_val.Length > 0) {
                    if (i%2 == 0) {
                        key = basket_val;
                    } else {
                        val = (float)System.Convert.ToSingle((string) basket_val);
                        cleanBasket.Add((string) key, (float) val);
                    }
                    i++;
                }
            }
            return cleanBasket;
        }

        public float do_fruitCalc(string the_input)
        {
            var items = new Dictionary<string, float>();
            string[] strTokens1 = the_input.Split(new char[1]{';'});
            List<Dictionary<string,float>> basketList = new List<Dictionary<string,float>>();
            var the_promos = new Dictionary<string,float>();

            foreach (string Tok in strTokens1) {
                string Tok_trimmed = Tok.Trim();
                string[] key_tokens = Tok_trimmed.Split(new char[1]{':'});
                string val = (string) key_tokens.GetValue(0);
                val = val.Trim();
                string val2 = (string) key_tokens.GetValue(key_tokens.Length-1);
                if (val == "Basket") {
                    string[] basket_tokens = val2.Split(new char[1]{','});
                    foreach (string basketTok in basket_tokens) {
                        string[] basket_vals = basketTok.Split(new char[1]{' '});
                        basketList.Add(this.clean_the_basket(basket_vals));
                    }
                    Console.WriteLine("Basket -->" + val2);
                } else if (val == "Promotions") {
                    string[] promo_toks = val2.Split(new char[1]{' '});
                    string val2a = (string) promo_toks.GetValue(promo_toks.Length-1);
                    if (Regex.IsMatch(val2a, @"[+-]?([0-9]+([.][0-9]*)?|[.][0-9]+)")) {
                        //Console.WriteLine("Promotions -->" + val2a);
                        the_promos = this.clean_the_basket(promo_toks);
                    }
                } else {
                    string[] price_tokens = Tok_trimmed.Split(new char[1]{' '});
                    string val3 = (string) price_tokens.GetValue(price_tokens.Length-1);
                    if (Regex.IsMatch(val3, @"[+-]?([0-9]+([.][0-9]*)?|[.][0-9]+)")) {
                        Console.WriteLine("Price -->" + price_tokens.GetValue(0) + ", " + val3);
                        string[] price_tokensa = val3.Split(new char[1]{'$'});
                        string price_key_raw = (string) price_tokens.GetValue(0);
                        string price_key = Regex.Replace(price_key_raw, @"[^a-zA-Z0-9 ]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));                        
                        items.Add(price_key, (float)System.Convert.ToSingle((string) price_tokensa.GetValue(price_tokensa.Length-1)));

                    }
                }
            }

            string d1 = string.Join(", ", items);
            Console.WriteLine("items --> { " + d1 + " }");

            string d2 = "[ ";
            int bTok_i = -1;
            foreach (Dictionary<string,float> bTok in basketList) {
                bTok_i = bTok_i + 2;
                string d2a = string.Join(", ", bTok);
                d2 = d2 + d2a;
                if (bTok_i < (basketList.Capacity-1)) {
                    d2 = d2 + ',';
                }
            }
            d2 = d2 + "]";
            Console.WriteLine("basket --> " + d2 + "");

            d1 = string.Join(", ", the_promos);
            Console.WriteLine("Promotions --> " + d1 + "");

            // Begin the real work...
            float total_price = (float) 0.0;
            foreach (Dictionary<string,float> aBasketItem in basketList) {
                foreach(var anItem in aBasketItem) {
                    float promo_discount = (float) 1.0;
                    if (items.ContainsKey(anItem.Key)) {
                        if (the_promos.ContainsKey(anItem.Key)) {
                            promo_discount = Extensions.GetValueOrDefault(the_promos, anItem.Key);
                        }

                        float price_each = Extensions.GetValueOrDefault(items, anItem.Key);
                        total_price = total_price + (anItem.Value * price_each * promo_discount);
                        Console.WriteLine(anItem.Key + " --> " + anItem.Value + " --> " + price_each.ToString() + " --> " + promo_discount.ToString() + " --> " + total_price.ToString());
                    }
                }
            }
            // End the real work...

            return (float) total_price;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            FruitCalc fcalc = new FruitCalc();

            var str_input1 = "Oranges – $10; Apples- $5 ; Promotions: No; Basket: Oranges - 5, Apples 1";
            float price1 = fcalc.do_fruitCalc(str_input1);
            var outString1 = price1.ToString("####0.00");
            Console.WriteLine(outString1);

            Console.WriteLine("--------------------------------------------------------------");

            var str_input2 = "Oranges – $10; Apples- $5 ; Promotions: Oranges – 0.5;  Basket: Oranges - 5, Apples 1";
            float price2 = fcalc.do_fruitCalc(str_input2);
            var outString2 = price2.ToString("####0.00");
            Console.WriteLine(outString2);

            Console.WriteLine("==============================================================");

            var str_input3 = "Oranges – $10; Apples- $5 ; Promotions: Oranges – 0.5, Apples- 1.0;  Basket: Oranges - 5, Apples 1";
            float price3 = fcalc.do_fruitCalc(str_input3);
            var outString3 = price3.ToString("####0.00");
            Console.WriteLine(outString3);

            Console.WriteLine("==============================================================");

            Console.WriteLine("We are done!");
        }
    }
}
