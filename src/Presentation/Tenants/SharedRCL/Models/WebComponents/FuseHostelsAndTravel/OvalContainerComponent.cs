namespace Travaloud.Tenants.SharedRCL.Models.WebComponents.FuseHostelsAndTravel;

public class OvalContainerComponent
{
	public string Id { get; private set; }
	public int? TopPosition { get; private set; }
	public int? BottomPosition { get; private set; }
	public int? LeftPosition { get; private set; }
	public int? RightPosition { get; private set; }

	public string Style
	{
		get
		{
			var style = "";

			if (TopPosition != null)
				style += $"top:{TopPosition.Value}%;";

			if (BottomPosition != null)
				style += $"bottom:{BottomPosition.Value}%;";

			if (LeftPosition != null)
				style += $"left:{LeftPosition.Value}%;";

			if (RightPosition != null)
				style += $"right:{RightPosition.Value}%;";

			style = style.Trim();

			return style;
		}
	}

	public OvalContainerComponent()
	{
	}

	public OvalContainerComponent(string id)
	{
		Id = id;
	}

	public OvalContainerComponent(string id, int? topPosition, int? bottomPosition, int? leftPosition, int? rightPosition) : this(id)
	{
		TopPosition = topPosition;
		BottomPosition = bottomPosition;
		LeftPosition = leftPosition;
		RightPosition = rightPosition;
	}
}