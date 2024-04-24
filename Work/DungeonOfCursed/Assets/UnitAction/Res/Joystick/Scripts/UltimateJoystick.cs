/* UltimateJoystick.cs */

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;


[ExecuteInEditMode]
public class UltimateJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

	RectTransform baseTrans;
	Vector2 defaultPos = Vector2.zero;
	Vector3 joystickCenter = Vector3.zero;
	int _inputId = -10;
	bool joystickReset = false;
	Rect joystickRect;
	CanvasGroup joystickGroup;
	float radius = 1.0f;


	public RectTransform joystickBase, joystick;
	public enum ScalingAxis
	{
		Width,
		Height
	}
	public ScalingAxis scalingAxis = ScalingAxis.Height;
	public enum Anchor
	{
		Left,
		Right
	}
	public Anchor anchor = Anchor.Left;
	public float activationRange = 1.0f;
	public bool customActivationRange = false;
	public float activationWidth = 50.0f, activationHeight = 75.0f;
	public float activationPositionHorizontal = 0.0f, activationPositionVertical = 0.0f;
	public float joystickSize = 2.5f, radiusModifier = 4.5f;
	public float positionHorizontal = 5.0f, positionVertical = 20.0f;

	
	public bool dynamicPositioning = false;
	public float gravity = 60.0f;
	bool gravityActive = false;
	public bool extendRadius = false;
	public enum Axis
	{
		Both,
		X,
		Y
	}
	public Axis axis = Axis.Both;
	public enum Boundary
	{
		Circular,
		Square
	}
	public Boundary boundary = Boundary.Circular;
	public float deadZone = 0.0f;
	public enum TapCountOption
	{
		NoCount,
		Accumulate,
		TouchRelease
	}
	public TapCountOption tapCountOption = TapCountOption.NoCount;
	public float tapCountDuration = 0.5f;
	public int targetTapCount = 2;
	float currentTapTime = 0.0f;
	int tapCount = 0;
	public bool useTouchInput = false;

	
	public bool disableVisuals = false;
	public bool inputTransition = false;
	public float transitionUntouchedDuration = 0.1f, transitionTouchedDuration = 0.1f;
	float transitionUntouchedSpeed, transitionTouchedSpeed;
	public bool useFade = false;
	public float fadeUntouched = 1.0f, fadeTouched = 0.5f;
	public bool useScale = false;
	public float scaleTouched = 0.9f;
	public bool showHighlight = false;
	public Color highlightColor = new Color( 1, 1, 1, 1 );
	public Image highlightBase, highlightJoystick;
	public bool showTension = false;
	public Color tensionColorNone = new Color( 1, 1, 1, 1 ), tensionColorFull = new Color( 1, 1, 1, 1 );
	public enum TensionType
	{
		Directional,
		Free
	}
	public TensionType tensionType = TensionType.Directional;
	public float rotationOffset = 0.0f;
	public float tensionDeadZone = 0.0f;
	public List<Image> TensionAccents = new List<Image>();
	

	static Dictionary<string,UltimateJoystick> UltimateJoysticks = new Dictionary<string, UltimateJoystick>();
	public string joystickName;
	bool joystickState = false;
	bool tapCountAchieved = false;


	public event Action OnPointerDownCallback, OnPointerUpCallback, OnDragCallback;
	public event Action OnUpdatePositioning;
	

	public enum JoystickTouchSize
	{
		Default,
		Medium,
		Large,
		Custom
	}
	[Header( "Depreciated Variables" )]
	public JoystickTouchSize joystickTouchSize = JoystickTouchSize.Default;
	public float customSpacing_X = -10, customSpacing_Y = -10;
	public float customTouchSize_X = -10, customTouchSize_Y = -10;
	public float customTouchSizePos_X = -10, customTouchSizePos_Y = -10;
	public RectTransform joystickSizeFolder;
	public Image tensionAccentUp, tensionAccentDown;
	public Image tensionAccentLeft, tensionAccentRight;


	void Awake ()
	{
		
		if( Application.isPlaying && joystickName != string.Empty )
		{
			
			if( UltimateJoysticks.ContainsKey( joystickName ) )
				UltimateJoysticks.Remove( joystickName );

			
			UltimateJoysticks.Add( joystickName, GetComponent<UltimateJoystick>() );
		}
	}

	void Start ()
	{
		
		if( !Application.isPlaying )
			return;

		
		UpdateJoystickPositioning();
		
		
		if( showHighlight )
			UpdateHighlightColor( highlightColor );

		
		if( showTension )
			TensionAccentReset();

		
		if( inputTransition )
		{
			
			joystickGroup = GetComponent<CanvasGroup>();

			
			if( joystickGroup == null )
				joystickGroup = baseTrans.gameObject.AddComponent<CanvasGroup>();

			
			transitionUntouchedSpeed = 1.0f / transitionUntouchedDuration;
			transitionTouchedSpeed = 1.0f / transitionTouchedDuration;
		}
		
		
		Transform parent = transform.parent;

		
		while( parent != null )
		{
			
			if( parent.transform.GetComponent<Canvas>() )
			{
				
				if( !parent.transform.GetComponent<UltimateJoystickScreenSizeUpdater>() )
				{
					
					parent.gameObject.AddComponent( typeof( UltimateJoystickScreenSizeUpdater ) );

					
					break;
				}

				
				break;
			}

			
			parent = parent.transform.parent;
		}

		
		if( useTouchInput )
			StartCoroutine( "ProcessTouchInput" );
	}


	public void OnPointerDown ( PointerEventData touchInfo )
	{
		if( useTouchInput )
			return;

		ProcessOnInputDown( touchInfo.position, touchInfo.pointerId );
	}

	public void OnDrag ( PointerEventData touchInfo )
	{
		if( useTouchInput )
			return;

		ProcessOnInputMoved( touchInfo.position, touchInfo.pointerId );
	}

	public void OnPointerUp ( PointerEventData touchInfo )
	{
		if( useTouchInput )
			return;

		ProcessOnInputUp( touchInfo.position, touchInfo.pointerId );
	}
	// END FOR UNITY EVENT SYSTEM //

	/// <summary>
	/// The coroutine will process the touch input if the user has the useTouchInput boolean enabled.
	/// </summary>
	IEnumerator ProcessTouchInput ()
	{
		
		while( useTouchInput )
		{
			
			if( Input.touchCount > 0 )
			{
				
				for( int fingerId = 0; fingerId < Input.touchCount; fingerId++ )
				{
					
					if( Input.GetTouch( fingerId ).phase == TouchPhase.Began )
					{
						
						if( joystickRect.Contains( Input.GetTouch( fingerId ).position ) )
							ProcessOnInputDown( Input.GetTouch( fingerId ).position, fingerId );
					}
					
					else if( Input.GetTouch( fingerId ).phase == TouchPhase.Moved )
						ProcessOnInputMoved( Input.GetTouch( fingerId ).position, fingerId );
					
					else if( Input.GetTouch( fingerId ).phase == TouchPhase.Ended || Input.GetTouch( fingerId ).phase == TouchPhase.Canceled )
						ProcessOnInputUp( Input.GetTouch( fingerId ).position, fingerId );
				}
			}
			
			else
			{
				
				if( !joystickReset )
					ResetJoystick();
			}
			
			yield return null;
		}
	}
	
	/// <summary>
	/// Processes the input when it has been initiated on the joystick.
	/// </summary>
	/// <param name="inputPosition">The position of the input on the screen.</param>
	/// <param name="inputId">The unique id of the input that has been initiated on the joystick.</param>
	void ProcessOnInputDown ( Vector2 inputPosition, int inputId )
	{
		
		if( joystickState )
			return;

		
		if( boundary == Boundary.Circular )
		{
			
			float distance = Vector2.Distance( joystick.position, inputPosition );

			
			if( ( distance / baseTrans.sizeDelta.x ) > 0.5f && joystickTouchSize != JoystickTouchSize.Custom )
				return;
		}

		
		joystickState = true;

		
		joystickReset = false;

		
		_inputId = inputId;

		
		if( gravity > 0 && gravityActive )
			StopCoroutine( "GravityHandler" );

		
		if( dynamicPositioning || disableVisuals )
		{
		
			joystickBase.position = inputPosition;

			
			joystickCenter = inputPosition;
		}

		
		if( inputTransition )
		{
			
			if( transitionUntouchedDuration > 0 || transitionTouchedDuration > 0 )
				StartCoroutine( "InputTransition" );
			
			else
			{
				
				if( useFade )
					joystickGroup.alpha = fadeTouched;

				
				if( useScale )
					joystickBase.localScale = Vector3.one * scaleTouched;
			}
		}

		
		if( tapCountOption != TapCountOption.NoCount )
		{
			
			if( tapCountOption == TapCountOption.Accumulate )
			{
				
				if( currentTapTime <= 0 )
				{
					
					tapCount = 1;
					StartCoroutine( "TapCountdown" );
				}
				
				else
					++tapCount;

				if( currentTapTime > 0 && tapCount >= targetTapCount )
				{
					
					currentTapTime = 0;

					
					StartCoroutine( "TapCountDelay" );
				}
			}
			
			else
				StartCoroutine( "TapCountdown" );
		}

		
		ProcessInput( inputPosition );

		
		if( OnPointerDownCallback != null )
			OnPointerDownCallback();
	}

	/// <summary>
	/// Processes the input when it has been moved on the screen.
	/// </summary>
	/// <param name="inputPosition">The position of the input on the screen.</param>
	/// <param name="inputId">The unique id of the input being sent in to this function.</param>
	void ProcessOnInputMoved ( Vector2 inputPosition, int inputId )
	{
		
		if( inputId != _inputId )
			return;

		
		ProcessInput( inputPosition );

		
		if( OnDragCallback != null )
			OnDragCallback();
	}

	/// <summary>
	/// Processes the input when it has been released.
	/// </summary>
	/// <param name="inputPosition">The position of the input on the screen.</param>
	/// <param name="inputId">The unique id of the input being sent into this function.</param>
	void ProcessOnInputUp ( Vector2 inputPosition, int inputId )
	{
		
		if( inputId != _inputId )
			return;

		
		joystickState = false;
		_inputId = -10;

		
		if( dynamicPositioning || disableVisuals || extendRadius )
		{
			
			joystickBase.position = defaultPos;

			
			joystickCenter = joystickBase.position;
		}

		
		if( gravity > 0 && gravity < 60 )
			StartCoroutine( "GravityHandler" );
		
		else
			joystick.anchoredPosition = Vector2.zero;

		
		if( showTension && ( gravity <= 0 || gravity >= 60 ) )
			TensionAccentReset();

		
		if( inputTransition && ( transitionTouchedDuration <= 0 && transitionUntouchedDuration <= 0 ) )
		{
			
			if( useFade )
				joystickGroup.alpha = fadeUntouched;

			
			if( useScale )
				joystickBase.localScale = Vector3.one;
		}

		
		if( tapCountOption == TapCountOption.TouchRelease )
		{
			
			if( currentTapTime > 0 )
				StartCoroutine( "TapCountDelay" );

			
			currentTapTime = 0;
		}

		
		UpdatePositionValues();

		
		if( OnPointerUpCallback != null )
			OnPointerUpCallback();
	}

	/// <summary>
	/// Processes the input provided and moves the joystick accordingly.
	/// </summary>
	/// <param name="inputPosition">The current position of the input.</param>
	void ProcessInput ( Vector2 inputPosition )
	{
		
		Vector2 tempVector = inputPosition - ( Vector2 )joystickCenter;

		
		if( axis == Axis.X )
			tempVector.y = 0;
		else if( axis == Axis.Y )
			tempVector.x = 0;

		
		if( boundary == Boundary.Circular )
			tempVector = Vector2.ClampMagnitude( tempVector, radius );
		
		else if( boundary == Boundary.Square )
		{
			tempVector.x = Mathf.Clamp( tempVector.x, -radius, radius );
			tempVector.y = Mathf.Clamp( tempVector.y, -radius, radius );
		}

		
		joystick.transform.position = ( Vector2 )joystickCenter + tempVector;
		
		
		if( extendRadius )
		{
			
			Vector3 currentTouchPosition = inputPosition;

			
			if( axis != Axis.Both )
			{
				if( axis == Axis.X )
					currentTouchPosition.y = joystickCenter.y;
				else
					currentTouchPosition.x = joystickCenter.x;
			}
			
			float touchDistance = Vector3.Distance( joystickCenter, currentTouchPosition );

			
			if( touchDistance >= radius )
			{
				
				Vector2 joystickPosition = ( joystick.position - joystickCenter ) / radius;

				
				joystickBase.position += new Vector3( joystickPosition.x, joystickPosition.y, 0 ) * ( touchDistance - radius );

				
				joystickCenter = joystickBase.position;
			}
		}

		
		UpdatePositionValues();

		
		if( showTension )
			TensionAccentDisplay();
	}
	
	/// <summary>
	/// This function will configure the position of an image based on the size and positioning set by the user.
	/// </summary>
	/// <param name="imageSize">The size of the image for calculating the position of the rect transform.</param>
	/// <param name="rawPosition">The raw position values (0-100).</param>
	Vector2 ConfigureImagePosition ( Vector2 imageSize, Vector2 rawPosition )
	{
		
		Vector2 fixedCustomSpacing = rawPosition / 100;

		
		float positionSpacerX = Screen.width * fixedCustomSpacing.x - ( imageSize.x * fixedCustomSpacing.x );
		float positionSpacerY = Screen.height * fixedCustomSpacing.y - ( imageSize.y * fixedCustomSpacing.y );

		
		Vector2 tempVector;

		
		tempVector.x = anchor == Anchor.Left ? positionSpacerX : ( Screen.width - imageSize.x ) - positionSpacerX;
		tempVector.x += ( imageSize.x / 2 );
		
		
		tempVector.y = positionSpacerY + ( imageSize.y / 2 );

		
		return tempVector;
	}

	/// <summary>
	/// This function updates the joystick's position on the screen.
	/// </summary>
	void UpdateJoystickPositioning ()
	{
		
		if( joystickBase == null )
		{
			if( Application.isPlaying )
				Debug.LogError( "Ultimate Joystick\nThere are some needed components that are not currently assigned. Please check the Assigned Variables section and be sure to assign all of the components." );
			return;
		}

		
		float referenceSize = scalingAxis == ScalingAxis.Height ? Screen.height : Screen.width;

		
		float textureSize = referenceSize * ( joystickSize / 10 );

		
		if( baseTrans == null )
			baseTrans = GetComponent<RectTransform>();

		
		baseTrans.anchorMin = Vector2.zero;
		baseTrans.anchorMax = Vector2.zero;
		baseTrans.pivot = new Vector2( 0.5f, 0.5f );

		
		Vector2 imagePosition = ConfigureImagePosition( new Vector2( textureSize, textureSize ), new Vector2( positionHorizontal, positionVertical ) );

		
		if( customActivationRange )
		{
			
			float fixedFBPX = activationWidth / 100;
			float fixedFBPY = activationHeight / 100;

		
			baseTrans.sizeDelta = new Vector2( Screen.width * fixedFBPX, Screen.height * fixedFBPY );

			
			Vector2 imagePos = ConfigureImagePosition( baseTrans.sizeDelta, new Vector2( activationPositionHorizontal, activationPositionVertical ) );

			
			baseTrans.position = imagePos;
		}
		else
		{
			
			Vector2 tempVector = new Vector2( textureSize, textureSize );

			
			baseTrans.sizeDelta = tempVector * activationRange;

		
			baseTrans.position = imagePosition;
		}

		
		if( dynamicPositioning || disableVisuals || extendRadius )
			defaultPos = imagePosition;

		
		joystickBase.anchorMin = new Vector2( 0.5f, 0.5f );
		joystickBase.anchorMax = new Vector2( 0.5f, 0.5f );
		joystickBase.pivot = new Vector2( 0.5f, 0.5f );

		
		joystickBase.sizeDelta = new Vector2( textureSize, textureSize );
		joystickBase.position = imagePosition;

	
		radius = joystickBase.sizeDelta.x * ( radiusModifier / 10 );

		
		joystickCenter = joystickBase.position;

		
		if( inputTransition && joystickGroup == null )
		{
			joystickGroup = GetComponent<CanvasGroup>();
			if( joystickGroup == null )
				joystickGroup = gameObject.AddComponent<CanvasGroup>();
		}

		
		if( useTouchInput )
			joystickRect = new Rect( new Vector2( baseTrans.position.x - ( baseTrans.sizeDelta.x / 2 ), baseTrans.position.y - ( baseTrans.sizeDelta.y / 2 ) ), baseTrans.sizeDelta );
	}

	/// <summary>
	/// This function is called only when showTension is true, and only when the joystick is moving.
	/// </summary>
	void TensionAccentDisplay ()
	{
		
		if( TensionAccents.Count == 0 )
		{
			Debug.LogError( "Ultimate Joystick\nThere are no tension accent images assigned. This could be happening for several reasons, but all of them should be fixable in the Ultimate Joystick inspector." );
			return;
		}

		
		if( tensionType == TensionType.Directional )
		{
			
			Vector2 joystickAxis = ( joystick.position - joystickCenter ) / radius;

			
			if( joystickAxis.x > 0 )
			{
				
				if( TensionAccents[ 3 ] != null )
					TensionAccents[ 3 ].color = Color.Lerp( tensionColorNone, tensionColorFull, joystickAxis.x <= tensionDeadZone ? 0 : ( joystickAxis.x - tensionDeadZone ) / ( 1.0f - tensionDeadZone ) );

				
				if( TensionAccents[ 1 ] != null && TensionAccents[ 1 ].color != tensionColorNone )
					TensionAccents[ 1 ].color = tensionColorNone;
			}
			
			else
			{
				
				if( TensionAccents[ 1 ] != null )
					TensionAccents[ 1 ].color = Color.Lerp( tensionColorNone, tensionColorFull, Mathf.Abs( joystickAxis.x ) <= tensionDeadZone ? 0 : ( Mathf.Abs( joystickAxis.x ) - tensionDeadZone ) / ( 1.0f - tensionDeadZone ) );
				if( TensionAccents[ 3 ] != null && TensionAccents[ 3 ].color != tensionColorNone )
					TensionAccents[ 3 ].color = tensionColorNone;
			}

			
			if( joystickAxis.y > 0 )
			{
				
				if( TensionAccents[ 0 ] != null )
					TensionAccents[ 0 ].color = Color.Lerp( tensionColorNone, tensionColorFull, joystickAxis.y <= tensionDeadZone ? 0 : ( joystickAxis.y - tensionDeadZone ) / ( 1.0f - tensionDeadZone ) );

				
				if( TensionAccents[ 2 ] != null && TensionAccents[ 2 ].color != tensionColorNone )
					TensionAccents[ 2 ].color = tensionColorNone;
			}
			
			else
			{
				
				if( TensionAccents[ 2 ] != null )
					TensionAccents[ 2 ].color = Color.Lerp( tensionColorNone, tensionColorFull, Mathf.Abs( joystickAxis.y ) <= tensionDeadZone ? 0 : ( Mathf.Abs( joystickAxis.y ) - tensionDeadZone ) / ( 1.0f - tensionDeadZone ) );
				if( TensionAccents[ 0 ] != null && TensionAccents[ 0 ].color != tensionColorNone )
					TensionAccents[ 0 ].color = tensionColorNone;
			}
		}
		
		else
		{
			
			if( TensionAccents[ 0 ] == null )
			{
				Debug.LogError( "Ultimate Joystick\nThere are no tension accent images assigned. This could be happening for several reasons, but all of them should be fixable in the Ultimate Joystick inspector." );
				return;
			}

			
			float distance = GetDistance();

		
			TensionAccents[ 0 ].color = Color.Lerp( tensionColorNone, tensionColorFull, distance <= tensionDeadZone ? 0 : ( distance - tensionDeadZone ) / ( 1.0f - tensionDeadZone ) );

		
			TensionAccents[ 0 ].transform.rotation = Quaternion.Euler( 0, 0, ( Mathf.Atan2( VerticalAxis, HorizontalAxis ) * Mathf.Rad2Deg ) + rotationOffset - 90 );
		}
	}
	
	/// <summary>
	/// This function resets the tension image's colors back to default.
	/// </summary>
	void TensionAccentReset ()
	{
		
		for( int i = 0; i < TensionAccents.Count; i++ )
		{
		
			if( TensionAccents[ i ] == null )
				continue;

			
			TensionAccents[ i ].color = tensionColorNone;
		}

	
		if( tensionType == TensionType.Free && TensionAccents.Count > 0 && TensionAccents[ 0 ] != null )
			TensionAccents[ 0 ].transform.rotation = Quaternion.identity;
	}
	
	/// <summary>
	/// This function is for returning the joystick back to center for a set amount of time.
	/// </summary>
	IEnumerator GravityHandler ()
	{
		
		gravityActive = true;
		float speed = 1.0f / ( GetDistance() / gravity );

		
		Vector3 startJoyPos = joystick.position;

	
		for( float t = 0.0f; t < 1.0f && gravityActive; t += Time.deltaTime * speed )
		{
			
			joystick.position = Vector3.Lerp( startJoyPos, joystickCenter, t );

		
			if( showTension )
				TensionAccentDisplay();

			
			UpdatePositionValues();

			yield return null;
		}

		
		if( gravityActive )
		{
			
			joystick.position = joystickCenter;

			
			if( showTension )
				TensionAccentReset();

			
			UpdatePositionValues();
		}

		
		gravityActive = false;
	}

	/// <summary>
	/// This coroutine will handle the input transitions over time according to the users options.
	/// </summary>
	IEnumerator InputTransition ()
	{
		
		float currentAlpha = joystickGroup.alpha;
		float currentScale = joystickBase.localScale.x;

		
		if( float.IsInfinity( transitionTouchedSpeed ) )
		{
			
			if( useFade )
				joystickGroup.alpha = fadeTouched;

			
			if( useScale )
				joystickBase.localScale = Vector3.one * scaleTouched;
		}
		
		else
		{
			
			for( float transition = 0.0f; transition < 1.0f && joystickState; transition += Time.deltaTime * transitionTouchedSpeed )
			{
				
				if( useFade )
					joystickGroup.alpha = Mathf.Lerp( currentAlpha, fadeTouched, transition );

				
				if( useScale )
					joystickBase.localScale = Vector3.one * Mathf.Lerp( currentScale, scaleTouched, transition );

				yield return null;
			}

			
			if( joystickState )
			{
				if( useFade )
					joystickGroup.alpha = fadeTouched;

				if( useScale )
					joystickBase.localScale = Vector3.one * scaleTouched;
			}
		}

		
		while( joystickState )
			yield return null;

		
		currentAlpha = joystickGroup.alpha;
		currentScale = joystickBase.localScale.x;

		
		if( float.IsInfinity( transitionUntouchedSpeed ) )
		{
			if( useFade )
				joystickGroup.alpha = fadeUntouched;

			if( useScale )
				joystickBase.localScale = Vector3.one;
		}
		
		else
		{
			for( float transition = 0.0f; transition < 1.0f && !joystickState; transition += Time.deltaTime * transitionUntouchedSpeed )
			{
				if( useFade )
					joystickGroup.alpha = Mathf.Lerp( currentAlpha, fadeUntouched, transition );

				if( useScale )
					joystickBase.localScale = Vector3.one * Mathf.Lerp( currentScale, 1.0f, transition );
				yield return null;
			}

			
			if( !joystickState )
			{
				if( useFade )
					joystickGroup.alpha = fadeUntouched;

				if( useScale )
					joystickBase.localScale = Vector3.one;
			}
		}
	}

	/// <summary>
	/// This function counts down the tap count duration. The current tap time that is being modified is check within the input functions.
	/// </summary>
	IEnumerator TapCountdown ()
	{
		
		currentTapTime = tapCountDuration;
		while( currentTapTime > 0 )
		{
			
			currentTapTime -= Time.deltaTime;
			yield return null;
		}
	}

	/// <summary>
	/// This function delays for one frame so that it can be correctly referenced as soon as it is achieved.
	/// </summary>
	IEnumerator TapCountDelay ()
	{
		tapCountAchieved = true;
		yield return new WaitForEndOfFrame();
		tapCountAchieved = false;
	}
	
	/// <summary>
	/// This function updates the position values of the joystick so that they can be referenced.
	/// </summary>
	void UpdatePositionValues ()
	{
		
		Vector2 joystickPosition = ( joystick.position - joystickCenter ) / radius;

		
		if( GetDistance() <= deadZone )
		{
			
			joystickPosition.x = 0.0f;
			joystickPosition.y = 0.0f;
		}

		
		HorizontalAxis = joystickPosition.x;
		VerticalAxis = joystickPosition.y;
	}

	/// <summary>
	/// Returns with a confirmation about the existence of the targeted Ultimate Joystick.
	/// </summary>
	static bool JoystickConfirmed ( string joystickName )
	{
		if( !UltimateJoysticks.ContainsKey( joystickName ) )
		{
			Debug.LogWarning( "Ultimate Joystick\nNo Ultimate Joystick has been registered with the name: " + joystickName + "." );
			return false;
		}
		return true;
	}

	/// <summary>
	/// Resets the joystick position and input information and stops any coroutines that might have been running.
	/// </summary>
	void ResetJoystick ()
	{
		joystickReset = true;
		gravityActive = false;
		StopCoroutine( "GravityHandler" );

		
		joystickState = false;
		_inputId = -10;
		
		
		if( dynamicPositioning || disableVisuals || extendRadius )
		{
			
			joystickBase.position = defaultPos;

			
			joystickCenter = joystickBase.position;
		}
		
		joystick.position = joystickCenter;

		
		if( showTension )
			TensionAccentReset();
	}

	#if UNITY_EDITOR
	void Update ()
	{
		
		if( !Application.isPlaying )
			UpdateJoystickPositioning();
	}
	#endif

	/* --------------------------------------------- *** PUBLIC FUNCTIONS FOR THE USER *** --------------------------------------------- */
	/// <summary>
	/// Resets the joystick and updates the size and placement of the Ultimate Joystick. Useful for screen rotations, changing of screen size, or changing of size and placement options.
	/// </summary>
	public void UpdatePositioning ()
	{
		
		if( Application.isPlaying )
			ResetJoystick();

		
		UpdateJoystickPositioning();

		
		if( OnUpdatePositioning != null )
			OnUpdatePositioning();
	}
	
	/// <summary>
	/// Returns a float value between -1 and 1 representing the horizontal value of the Ultimate Joystick.
	/// </summary>
	public float GetHorizontalAxis ()
	{
		return HorizontalAxis;
	}

	/// <summary>
	/// Returns a float value between -1 and 1 representing the vertical value of the Ultimate Joystick.
	/// </summary>
	public float GetVerticalAxis ()
	{
		return VerticalAxis;
	}

	/// <summary>
	/// Returns a value of -1, 0 or 1 representing the raw horizontal value of the Ultimate Joystick.
	/// </summary>
	public float GetHorizontalAxisRaw ()
	{
		float temp = HorizontalAxis;

		if( Mathf.Abs( temp ) <= deadZone )
			temp = 0.0f;
		else
			temp = temp < 0.0f ? -1.0f : 1.0f;

		return temp;
	}

	/// <summary>
	/// Returns a value of -1, 0 or 1 representing the raw vertical value of the Ultimate Joystick.
	/// </summary>
	public float GetVerticalAxisRaw ()
	{
		float temp = VerticalAxis;
		if( Mathf.Abs( temp ) <= deadZone )
			temp = 0.0f;
		else
			temp = temp < 0.0f ? -1.0f : 1.0f;

		return temp;
	}

	/// <summary>
	/// Returns the current value of the horizontal axis.
	/// </summary>
	public float HorizontalAxis
	{
		get;
		private set;
	}

	/// <summary>
	/// Returns the current value of the vertical axis.
	/// </summary>
	public float VerticalAxis
	{
		get;
		private set;
	}

	/// <summary>
	/// Returns a float value between 0 and 1 representing the distance of the joystick from the base.
	/// </summary>
	public float GetDistance ()
	{
		return Vector3.Distance( joystick.position, joystickCenter ) / radius;
	}

	/// <summary>
	/// Updates the color of the highlights attached to the Ultimate Joystick with the targeted color.
	/// </summary>
	/// <param name="targetColor">New highlight color.</param>
	public void UpdateHighlightColor ( Color targetColor )
	{
		if( !showHighlight )
			return;

		highlightColor = targetColor;
		
		
		if( highlightBase != null )
			highlightBase.color = highlightColor;
		if( highlightJoystick != null )
			highlightJoystick.color = highlightColor;
	}

	/// <summary>
	/// Updates the colors of the tension accents attached to the Ultimate Joystick with the targeted colors.
	/// </summary>
	/// <param name="targetTensionNone">New idle tension color.</param>
	/// <param name="targetTensionFull">New full tension color.</param>
	public void UpdateTensionColors ( Color targetTensionNone, Color targetTensionFull )
	{
		if( !showTension )
			return;

		tensionColorNone = targetTensionNone;
		tensionColorFull = targetTensionFull;
	}

	/// <summary>
	/// Returns the current state of the Ultimate Joystick. This function will return true when the joystick is being interacted with, and false when not.
	/// </summary>
	public bool GetJoystickState ()
	{
		return joystickState;
	}

	/// <summary>
	/// Returns the tap count to the Ultimate Joystick.
	/// </summary>
	public bool GetTapCount ()
	{
		return tapCountAchieved;
	}

	/// <summary>
	/// Disables the Ultimate Joystick.
	/// </summary>
	public void DisableJoystick ()
	{
		
		joystickState = false;
		_inputId = -10;
		
		
		if( dynamicPositioning || disableVisuals || extendRadius )
		{
			joystickBase.position = defaultPos;
			joystickCenter = joystickBase.position;
		}
		
		
		joystick.position = joystickCenter;

		
		UpdatePositionValues();
		
		
		if( showTension )
			TensionAccentReset();

		
		if( inputTransition )
		{
			
			if( useFade )
				joystickGroup.alpha = fadeUntouched;

			
			if( useScale )
				joystickBase.transform.localScale = Vector3.one;
		}
		
		
		gameObject.SetActive( false );
	}

	/// <summary>
	/// Enables the Ultimate Joystick.
	/// </summary>
	public void EnableJoystick ()
	{
		
		joystick.position = joystickCenter;

		
		gameObject.SetActive( true );
	}

	/// <summary>
	/// Returns the Ultimate Joystick of the targeted name if it exists within the scene.
	/// </summary>
	/// <param name="joystickName">The Joystick Name of the desired Ultimate Joystick.</param>
	public static UltimateJoystick GetUltimateJoystick ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return null;

		return UltimateJoysticks[ joystickName ];
	}

	/// <summary>
	/// Returns a float value between -1 and 1 representing the horizontal value of the Ultimate Joystick.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static float GetHorizontalAxis ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return 0.0f;

		return UltimateJoysticks[ joystickName ].GetHorizontalAxis();
	}

	/// <summary>
	/// Returns a float value between -1 and 1 representing the vertical value of the Ultimate Joystick.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static float GetVerticalAxis ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return 0.0f;

		return UltimateJoysticks[ joystickName ].GetVerticalAxis();
	}

	/// <summary>
	/// Returns a value of -1, 0 or 1 representing the raw horizontal value of the Ultimate Joystick.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static float GetHorizontalAxisRaw ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return 0.0f;

		return UltimateJoysticks[ joystickName ].GetHorizontalAxisRaw();
	}

	/// <summary>
	/// Returns a value of -1, 0 or 1 representing the raw vertical value of the Ultimate Joystick.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static float GetVerticalAxisRaw ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return 0.0f;

		return UltimateJoysticks[ joystickName ].GetVerticalAxisRaw();
	}

	/// <summary>
	/// Returns a float value between 0 and 1 representing the distance of the joystick from the base.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static float GetDistance ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return 0.0f;

		return UltimateJoysticks[ joystickName ].GetDistance();
	}

	/// <summary>
	/// Returns the current interaction state of the Ultimate Joystick.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static bool GetJoystickState ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return false;

		return UltimateJoysticks[ joystickName ].joystickState;
	}

	/// <summary>
	/// Returns the current state of the tap count according to the options set.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static bool GetTapCount ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return false;

		return UltimateJoysticks[ joystickName ].GetTapCount();
	}

	/// <summary>
	/// Disables the targeted Ultimate Joystick.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static void DisableJoystick ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return;

		UltimateJoysticks[ joystickName ].DisableJoystick();
	}

	/// <summary>
	/// Enables the targeted Ultimate Joystick.
	/// </summary>
	/// <param name="joystickName">The name of the desired Ultimate Joystick.</param>
	public static void EnableJoystick ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return;

		UltimateJoysticks[ joystickName ].EnableJoystick();
	}

}