namespace ThoughtWorks.CruiseControl.UnitTests.Logging
{
	public class CircularArrayTestResources
	{
		private CircularArrayTestResources()
		{
		}

		public static readonly string SingleLine = 
			@"1. This is a test";

		public static readonly string ForwardFive = 
			@"1. This is a test
2. This is a test
3. This is a test
4. This is a test
5. This is a test";

		public static readonly string ForwardThirty = 
			@"1. This is a test
2. This is a test
3. This is a test
4. This is a test
5. This is a test
6. This is a test
7. This is a test
8. This is a test
9. This is a test
10. This is a test
11. This is a test
12. This is a test
13. This is a test
14. This is a test
15. This is a test
16. This is a test
17. This is a test
18. This is a test
19. This is a test
20. This is a test";

		public static readonly string ForwardLastFive = 
			@"16. This is a test
17. This is a test
18. This is a test
19. This is a test
20. This is a test";
	
		public static readonly string BackwardFive =
			@"5. This is a test
4. This is a test
3. This is a test
2. This is a test
1. This is a test";

		public static readonly string BackwardThirty = 
			@"20. This is a test
19. This is a test
18. This is a test
17. This is a test
16. This is a test
15. This is a test
14. This is a test
13. This is a test
12. This is a test
11. This is a test
10. This is a test
9. This is a test
8. This is a test
7. This is a test
6. This is a test
5. This is a test
4. This is a test
3. This is a test
2. This is a test
1. This is a test";

		public static readonly string BackwardLastFive = 
			@"20. This is a test
19. This is a test
18. This is a test
17. This is a test
16. This is a test";
	}		
}