using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.draco18s.artificer.quests.hero {
	public class HarrowDeck {
		public static List<Card> deck;
		private static int drawIndex = 0;
		private static Random rnd = new Random();
		static HarrowDeck() {
			deck = new List<Card>();
			for(int i = 0; i < 36; i++) {
				deck.Add(new Card(i));
			}
			resetAndShuffle();
		}

		public static Card pullCard() {
			drawIndex++;
			if(drawIndex > deck.Count) {
				resetAndShuffle();
				drawIndex++;
			}

			return deck[drawIndex - 1];
		}

		public static void resetAndShuffle() {
			drawIndex = 0;
			deck = deck.OrderBy(x => rnd.Next()).ToList();
		}


		public readonly Fortune nature;
		public readonly Fortune nurture;
		public readonly Fortune mind;
		public readonly Fortune body;
		public readonly Fortune spirit;

		public readonly Fortune STR;
		public readonly Fortune AGL;
		public readonly Fortune INT;
		public readonly Fortune CHA;
		public HarrowDeck() {
			nature = new Fortune(pullCard(), 9);
			nurture = new Fortune(pullCard(), 3);
			mind = new Fortune(pullCard(), 0);
			body = new Fortune(pullCard(), 0);
			spirit = new Fortune(pullCard(), 4);
			STR = new Fortune(pullCard(), 8);
			AGL = new Fortune(pullCard(), 8);
			INT = new Fortune(pullCard(), 8);
			CHA = new Fortune(pullCard(), 8);
			resolve(this);
		}

		public Fortune getAttr(Suit s) {
			switch(s) {
				case Suit.HAMMER_STR:
					return this.STR;
				case Suit.KEY_AGL:
					return this.AGL;
				case Suit.TOME_INT:
					return this.INT;
				case Suit.CROWN_CHA:
					return this.CHA;
			}
			return null;
		}

		private static void resolve(HarrowDeck deck) {
			Suit s;
			int n;
			s = deck.nature.card.getSuit();
			n = deck.nature.card.getValue();
			resolveNature(deck, s, n);
			//UnityEngine.Debug.Log("1: " + deck.body.tokens + "," + deck.mind.tokens);
			//UnityEngine.Debug.Log("0: " + deck.STR.tokens + "," + deck.AGL.tokens + "," + deck.INT.tokens + "," + deck.CHA.tokens + "(" + deck.nature.tokens + ")");
			deck.nature.tokens = 0;
			s = deck.spirit.card.getSuit();
			n = deck.spirit.card.getValue();
			resolveSpirit(deck, s, n);
			//UnityEngine.Debug.Log("1: " + deck.STR.tokens + "," + deck.AGL.tokens + "," + deck.INT.tokens + "," + deck.CHA.tokens + "(" + deck.spirit.tokens + ")");
			deck.spirit.tokens = 0;
			s = deck.body.card.getSuit();
			n = deck.body.card.getValue();
			resolveBody(deck, s, n);
			//UnityEngine.Debug.Log("2: " + deck.STR.tokens + "," + deck.AGL.tokens + "," + deck.INT.tokens + "," + deck.CHA.tokens + "(" + deck.body.tokens + ")");
			deck.body.tokens = 0;
			s = deck.mind.card.getSuit();
			n = deck.mind.card.getValue();
			resolveMind(deck, s, n);
			//UnityEngine.Debug.Log(deck.STR.tokens + "," + deck.AGL.tokens + "," + deck.INT.tokens + "," + deck.CHA.tokens + "(" + deck.mind.tokens + ")");
			deck.mind.tokens = 0;
			s = deck.nurture.card.getSuit();
			n = deck.nurture.card.getValue();
			s = resolveNurture(deck, s, n, true);
			//UnityEngine.Debug.Log(deck.STR.tokens + "," + deck.AGL.tokens + "," + deck.INT.tokens + "," + deck.CHA.tokens + "(" + deck.nurture.tokens + ")");
			deck.nurture.tokens = 0;
			//str, agl, int, cha in order starting with 's's
			for(int o = 0; o < 4; o++) {
				Suit ss = (Suit)((int)(s + o) % 4);
				resolveAttribute(deck, ss);
			}
			//UnityEngine.Debug.Log(deck.STR.tokens + "," + deck.AGL.tokens + "," + deck.INT.tokens + "," + deck.CHA.tokens);
			
		}

		private static void resolveNature(HarrowDeck deck, Suit s, int n) {
			switch(s) {
				case Suit.HAMMER_STR:
				case Suit.KEY_AGL:
					if(n != 5) {
						deck.body.tokens += deck.nature.tokens / 3 * 2;
						deck.mind.tokens += deck.nature.tokens / 3;
					}
					else {
						deck.body.tokens += deck.nature.tokens / 3;
						deck.mind.tokens += deck.nature.tokens / 3;
						switch(s) {
							case Suit.HAMMER_STR:
								deck.mind.tokens += deck.STR.tokens / 3;
								break;
							case Suit.KEY_AGL:
								deck.mind.tokens += deck.AGL.tokens / 3;
								break;
						}
					}
					break;
				case Suit.TOME_INT:
				case Suit.CROWN_CHA:
					if(n != 5) {
						deck.body.tokens += deck.nature.tokens / 3;
						deck.mind.tokens += deck.nature.tokens / 3 * 2;
					}
					else {
						deck.body.tokens += deck.nature.tokens / 3;
						deck.mind.tokens += deck.nature.tokens / 3;
						switch(s) {
							case Suit.TOME_INT:
								deck.mind.tokens += deck.INT.tokens / 3;
								break;
							case Suit.CROWN_CHA:
								deck.mind.tokens += deck.CHA.tokens / 3;
								break;
						}
					}
					break;
			}
		}
		private static void resolveSpirit(HarrowDeck deck, Suit s, int n) {
			//UnityEngine.Debug.Log(s);
			switch(s) {
				case Suit.HAMMER_STR:
					deck.STR.tokens += deck.spirit.tokens / 2;
					break;
				case Suit.KEY_AGL:
					deck.AGL.tokens += deck.spirit.tokens / 2;
					break;
				case Suit.TOME_INT:
					deck.INT.tokens += deck.spirit.tokens / 2;
					break;
				case Suit.CROWN_CHA:
					deck.CHA.tokens += deck.spirit.tokens / 2;
					break;
			}
			s += ((n % 3) + 1);
			s = (Suit)((int)s % 4);
			//UnityEngine.Debug.Log(s);
			switch(s) {
				case Suit.HAMMER_STR:
					deck.STR.tokens += deck.spirit.tokens / 2;
					break;
				case Suit.KEY_AGL:
					deck.AGL.tokens += deck.spirit.tokens / 2;
					break;
				case Suit.TOME_INT:
					deck.INT.tokens += deck.spirit.tokens / 2;
					break;
				case Suit.CROWN_CHA:
					deck.CHA.tokens += deck.spirit.tokens / 2;
					break;
			}
		}
		private static void resolveBody(HarrowDeck deck, Suit s, int n) {
			//UnityEngine.Debug.Log(s + "," + n);
			if((s == Suit.HAMMER_STR || s == Suit.KEY_AGL) && (n == 1 || n == 3 || n == 7 || n == 9)) {
				//ideologue
				switch(s) {
					case Suit.HAMMER_STR:
						deck.STR.tokens += deck.body.tokens;
						break;
					case Suit.KEY_AGL:
						deck.AGL.tokens += deck.body.tokens;
						break;
				}
			}
			else if(n == 2) {
				//equalizer
				switch(s) {
					case Suit.HAMMER_STR:
					case Suit.KEY_AGL:
					case Suit.TOME_INT:
					case Suit.CROWN_CHA:
						deck.STR.tokens += deck.body.tokens / 2;
						deck.AGL.tokens += deck.body.tokens / 2;
						break;
				}
			}
			else {
				switch(s) {
					case Suit.HAMMER_STR:
						deck.STR.tokens += deck.body.tokens / 3 * 2;
						deck.AGL.tokens += deck.body.tokens / 3;
						break;
					case Suit.KEY_AGL:
						deck.STR.tokens += deck.body.tokens / 3;
						deck.AGL.tokens += deck.body.tokens / 3 * 2;
						break;
					case Suit.TOME_INT:
						deck.STR.tokens += deck.body.tokens / 3 * 2;
						deck.AGL.tokens += deck.body.tokens / 3;
						break;
					case Suit.CROWN_CHA:
						deck.STR.tokens += deck.body.tokens / 3;
						deck.AGL.tokens += deck.body.tokens / 3 * 2;
						break;
				}
			}
		}
		private static void resolveMind(HarrowDeck deck, Suit s, int n) {
			if((s == Suit.TOME_INT || s == Suit.CROWN_CHA) && (n == 1 || n == 3 || n == 7 || n == 9)) {
				//ideologue
				switch(s) {
					case Suit.TOME_INT:
						deck.INT.tokens += deck.mind.tokens;
						break;
					case Suit.CROWN_CHA:
						deck.CHA.tokens += deck.mind.tokens;
						break;
				}
			}
			else if(n == 2) {
				//equalizer
				switch(s) {
					case Suit.HAMMER_STR:
					case Suit.KEY_AGL:
					case Suit.TOME_INT:
					case Suit.CROWN_CHA:
						deck.INT.tokens += deck.mind.tokens / 2;
						deck.CHA.tokens += deck.mind.tokens / 2;
						break;
				}
			}
			else {
				switch(s) {
					case Suit.HAMMER_STR:
						deck.INT.tokens += deck.mind.tokens / 3;
						deck.CHA.tokens += deck.mind.tokens / 3 * 2;
						break;
					case Suit.KEY_AGL:
						deck.INT.tokens += deck.mind.tokens / 3 * 2;
						deck.CHA.tokens += deck.mind.tokens / 3;
						break;
					case Suit.TOME_INT:
						deck.INT.tokens += deck.mind.tokens / 3;
						deck.CHA.tokens += deck.mind.tokens / 3 * 2;
						break;
					case Suit.CROWN_CHA:
						deck.INT.tokens += deck.mind.tokens / 3 * 2;
						deck.CHA.tokens += deck.mind.tokens / 3;
						break;
				}
			}
		}

		private static Suit resolveNurture(HarrowDeck deck, Suit s, int n, bool recurse) {
			//UnityEngine.Debug.Log(s + "," + n + "[" + recurse + "]");
			Suit r = s;
			if(n == 6) {
				//anarchist
				if(recurse) {
					if(s == Suit.HAMMER_STR || s == Suit.KEY_AGL) {
						r = resolveNurture(deck, deck.nature.card.getSuit(), deck.nature.card.getValue(), false);
					}
					else {
						r = resolveNurture(deck, deck.spirit.card.getSuit(), deck.spirit.card.getValue(), false);
					}
				}
				else {
					switch(s) {
						case Suit.HAMMER_STR:
							deck.STR.tokens += deck.nurture.tokens;
							break;
						case Suit.KEY_AGL:
							deck.AGL.tokens += deck.nurture.tokens;
							break;
						case Suit.TOME_INT:
							deck.INT.tokens += deck.nurture.tokens;
							break;
						case Suit.CROWN_CHA:
							deck.CHA.tokens += deck.nurture.tokens;
							break;
					}
				}
			}
			else if(n == 4) {
				//traditionalist
				if(recurse) {
					if(s == Suit.HAMMER_STR || s == Suit.KEY_AGL) {
						r = resolveNurture(deck, deck.body.card.getSuit(), deck.body.card.getValue(), false);
					}
					else {
						r = resolveNurture(deck, deck.mind.card.getSuit(), deck.mind.card.getValue(), false);
					}
				}
				else {
					switch(s) {
						case Suit.HAMMER_STR:
							deck.STR.tokens += deck.nurture.tokens;
							break;
						case Suit.KEY_AGL:
							deck.AGL.tokens += deck.nurture.tokens;
							break;
						case Suit.TOME_INT:
							deck.INT.tokens += deck.nurture.tokens;
							break;
						case Suit.CROWN_CHA:
							deck.CHA.tokens += deck.nurture.tokens;
							break;
					}
				}
			}
			else {
				switch(s) {
					case Suit.HAMMER_STR:
						deck.STR.tokens += deck.nurture.tokens;
						break;
					case Suit.KEY_AGL:
						deck.AGL.tokens += deck.nurture.tokens;
						break;
					case Suit.TOME_INT:
						deck.INT.tokens += deck.nurture.tokens;
						break;
					case Suit.CROWN_CHA:
						deck.CHA.tokens += deck.nurture.tokens;
						break;
				}
			}
			return r;
		}

		/*public static void _resolveAttribute(HarrowDeck deck, Suit attr) {
			resolveAttribute(deck, attr);
		}*/

		private static void resolveAttribute(HarrowDeck deck, Suit attr) {
			//UnityEngine.Debug.Log("Resolving " + attr);
			Fortune f = deck.getAttr(attr);
			Suit s = f.card.getSuit();
			int n = f.card.getValue();
			//UnityEngine.Debug.Log("    " + s + n);
			switch(n) {
				case 1:
					switch(s) {
						case Suit.HAMMER_STR:
						case Suit.TOME_INT:
							stealGive(deck, attr, 3, -1);
							break;
						case Suit.KEY_AGL: //steal 1 from all
							stealGive(deck, attr, 1, -1);
							stealGive(deck, attr, 2, -1);
							stealGive(deck, attr, 3, -1);
							break;
						case Suit.CROWN_CHA: //give 1 to all
							stealGive(deck, attr, 1, 1);
							stealGive(deck, attr, 2, 1);
							stealGive(deck, attr, 3, 1);
							break;
					}
					break;
				case 2:
					stealGive(deck, attr, 1, 1);
					break;
				case 3:
					stealGive(deck, attr, 2, 1);
					break;
				case 4:
					stealGive(deck, attr, 3, 1);
					break;
				case 5: //steal from hammers&keys or tomes&crowns
					bool getHammerKey = false;
					switch(s) {
						case Suit.HAMMER_STR:
						case Suit.CROWN_CHA:
							getHammerKey = true;
							break;
						case Suit.TOME_INT:
						case Suit.KEY_AGL:
							getHammerKey = false;
							break;
					}
					for(Suit t = Suit.HAMMER_STR; t <= Suit.CROWN_CHA; t++) {
						Fortune a = deck.getAttr(t);
						Suit d = a.card.getSuit();
						if(d == Suit.HAMMER_STR || d == Suit.KEY_AGL) {
							if(getHammerKey) {
								a.tokens--;
								f.tokens++;
							}
						}
						else {
							if(!getHammerKey) {
								a.tokens--;
								f.tokens++;
							}
						}
					}
					break;
				case 6:
					//UnityEngine.Debug.Log("    !" + attr);
					//UnityEngine.Debug.Log("    [" + deck.STR.tokens + "," + deck.AGL.tokens + "," + deck.INT.tokens + "," + deck.CHA.tokens + "]");
					List<Fortune> arr = new List<Fortune>();
					for(Suit t = Suit.HAMMER_STR; t <= Suit.CROWN_CHA; t++) {
						Fortune fff = deck.getAttr(t);
						arr.Add(fff);
					}
					arr.Sort((x, y) => x.tokens.CompareTo(y.tokens));
					bool cont = true;
					Fortune toActOn;
					int i = 0;
					switch(s) {
						case Suit.HAMMER_STR:
						case Suit.CROWN_CHA:
							do {
								toActOn = arr[i];
								i++;
								if(i > 3 || (toActOn.tokens != arr[i].tokens && toActOn != f)) {
									cont = false;
								}
								if(i <= 3 && toActOn.tokens == arr[i].tokens && toActOn != f) {
									i++;
									if(i > 3) cont = false;
								}
							} while(cont);
							//UnityEngine.Debug.Log(s + ", Lowest: " + (Suit)(i-1));
							if(s == Suit.HAMMER_STR) {
								toActOn.tokens -= 2;
								f.tokens += 2;
							}
							else {
								toActOn.tokens += 2;
								f.tokens -= 2;
							}
							break;
						case Suit.KEY_AGL:
						case Suit.TOME_INT:
							i = 3;
							do {
								toActOn = arr[i];
								i--;
								if(i > 0 || (toActOn.tokens != arr[i].tokens && toActOn != f)) {
									cont = false;
								}
								if(i > 0 && toActOn.tokens == arr[i].tokens && toActOn != f) {
									i--;
									if(i < 0) cont = false;
								}
							} while(cont);
							//UnityEngine.Debug.Log(s + ", Highest: " + (Suit)(i - 1));
							if(s == Suit.HAMMER_STR) {
								toActOn.tokens -= 2;
								f.tokens += 2;
							}
							else {
								toActOn.tokens += 2;
								f.tokens -= 2;
							}
							break;
					}
					break;
				case 7: //give to hammer&key or tome&crown
					bool getHammerKey2 = false;
					switch(s) {
						case Suit.HAMMER_STR:
						case Suit.CROWN_CHA:
							getHammerKey2 = true;
							break;
						case Suit.TOME_INT:
						case Suit.KEY_AGL:
							getHammerKey2 = false;
							break;
					}
					for(Suit t = Suit.HAMMER_STR; t <= Suit.CROWN_CHA; t++) {
						Fortune a = deck.getAttr(t);
						Suit d = a.card.getSuit();
						if(d == Suit.HAMMER_STR || d == Suit.KEY_AGL) {
							if(getHammerKey2) {
								a.tokens++;
								f.tokens--;
							}
						}
						else {
							if(!getHammerKey2) {
								a.tokens++;
								f.tokens--;
							}
						}
					}
					break;
				case 8:
					stealGive(deck, attr, 1, -1);
					break;
				case 9:
					stealGive(deck, attr, 2, -1);
					break;
			}
		}

		private static void stealGive(HarrowDeck deck, Suit v, int dist, int num) {
			Fortune f1 = deck.getAttr(v);
			Fortune f2 = deck.getAttr((Suit)((int)(v+dist)%4));

			f1.tokens -= num;
			f2.tokens += num;
		}
	}

	public class Fortune {
		public readonly Card card;
		public int tokens;

		public Fortune(Card c, int n) {
			card = c;
			tokens = n;
		}
	}

	public class Card {
		public readonly int cNum;
		public Card(int n) {
			cNum = n;
		}

		public Suit getSuit() {
			int v = cNum - (cNum % 9);
			return (Suit)(v/9);
		}

		public int getValue() {
			return cNum%9;
		}
	}

	public enum Suit {
		HAMMER_STR,
		KEY_AGL,
		//SHIELD_CON,
		TOME_INT,
		//STAR_WIS,
		CROWN_CHA
	}
}
