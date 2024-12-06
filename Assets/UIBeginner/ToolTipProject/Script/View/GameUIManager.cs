using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class GameUIManager : MonoSingleton<GameUIManager>
{
	public enum LayerUse
	{
		MainView = 0,
		Config = 1,
		DynamicUI = 2
	}
	public static Dictionary<string, VisualElement> helpers = new Dictionary<string, VisualElement>();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	static void Init()
	{
		helpers = new Dictionary<string, VisualElement>();    
	}

	public static void AddHelper(string name, VisualElement helper)
	{
		helpers.Add(name, helper);
	}

	public static VisualElement GetHelper(string name)
	{
		return helpers[name];
	}

	public static void ChangeAllHelperOpacity(float opacity)
	{
		foreach (var helper in helpers.Values)
		{
			helper.style.opacity = opacity;
		}
	}

	UIDocument mainUIDocument;
	VisualElement root;
	List<VisualElement> layers;

	private MainView mainView;
	private ConfigView configView; 
	[SerializeField] private VisualTreeAsset configMenuVTA;
	VisualElement configMenu;

	public MainView MainView { get => mainView; set => mainView = value; }
	public ConfigView ConfigView { get => configView; set => configView = value; }
	public List<VisualElement> Layers { get => layers; set => layers = value; }

	private void Awake() 
	{
		EnhancedTouchSupport.Enable();
		mainUIDocument = GetComponent<UIDocument>();
		root = mainUIDocument.rootVisualElement;

		layers = root.Query<VisualElement>(classes: "layer").ToList();
		layers.Sort((ve1, ve2) => ve1.name.CompareTo(ve2.name));
		InitDefaultLayer();
		AddLayerEvent();

		HandleSafeArea();

		GetViewComponents();
		InstantiateView();
		InitViewComponents();
		
		/* Testing */
		TestDynamicUI();
	}

	private void GetViewComponents()
	{
		mainView = GetComponent<MainView>();
		configView = GetComponent<ConfigView>();
		mainView.GameUIManager = configView.GameUIManager = this;
	}

	private void InstantiateView()
	{
		configMenu = configMenuVTA.Instantiate();
		configMenu.name = "config__menu-root";
		configMenu.style.flexGrow = 1;
		layers[1].Q(classes:"safe-area").Add(configMenu);
	}


	private void InitViewComponents()
	{
		mainView.Init();
		configView.Init();
	}

	public void HandleSafeArea()
	{
		/* Calculate the safe area so UIs don't touch unreachable parts of the screen */
		Rect safeArea = Screen.safeArea;
		Vector2 leftTop = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(safeArea.xMin, Screen.height - safeArea.yMax));
		Vector2 rightBottom = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(Screen.width - safeArea.xMax, safeArea.yMin));
		root.Query<VisualElement>(classes: "safe-area").ForEach((ve) => 
		{
			ve.style.paddingLeft = leftTop.x;
			ve.style.paddingTop = leftTop.y;
			ve.style.paddingRight = rightBottom.x;
			ve.style.paddingBottom = rightBottom.y;
			/*  */
		});
	}

	public void InitDefaultLayer()
	{
		layers[0].style.left = 0;
		layers[0].style.top = 0;

		for (int i = 1; i < layers.Count; i++)
		{
			layers[i].style.left = 99999f;
			layers[i].style.top = 99999f;
		}
	}

	public void AddLayerEvent()
	{
		layers[(int)LayerUse.MainView].RegisterCallback<PointerDownEvent>((evt) => 
		{
			Touch touch = TouchExtension.GetTouchOverlapRect(new Rect(evt.position, new Vector2(1, 1)), layers[0].panel);
			StartCoroutine(MainViewSwipeHandle(touch));
		});
	}

	public void ActivateLayer(int layerIndex)
	{
		layers[layerIndex].style.left = 0;
		layers[layerIndex].style.top = 0;
	}

	public void DeactivateLayer(int layerIndex)
	{
		layers[layerIndex].style.left = 99999f;
		layers[layerIndex].style.top = 99999f;
	}

	public VisualElement GetLayer(int layerIndex)
	{
		return layers[layerIndex];
	}

	public delegate void MainViewSwipeDelegate(Vector2 vector2);
	public MainViewSwipeDelegate mainViewSwipeDelegate;
	Vector2 previousSwipePosition, swipeVector;
	public IEnumerator MainViewSwipeHandle(Touch touch)
	{
		previousSwipePosition = touch.screenPosition;
		while (touch.phase != UnityEngine.InputSystem.TouchPhase.Ended)
		{
			swipeVector = touch.screenPosition - previousSwipePosition;
			mainViewSwipeDelegate?.Invoke(swipeVector);
			previousSwipePosition = touch.screenPosition;
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}
	
	/* Dynamic UI parts */
	VisualElement dynamicUIButton;
	Dictionary<int, VisualElement> dynamicUIDictionary = new Dictionary<int, VisualElement>();
	List<VisualElement> moveableElements;
	void TestDynamicUI()
	{
		dynamicUIButton = root.Q<VisualElement>(name: "dynamic-ui-button");
		var dynamicLayer = layers[(int)LayerUse.DynamicUI];
		dynamicUIButton.RegisterCallback<PointerDownEvent>((evt) => 
		{
			evt.StopPropagation();
			ActivateLayer((int)LayerUse.DynamicUI);
		});
		
		moveableElements = root.Query<VisualElement>(classes: "dynamic-ui__movable").ToList();
		
		for (int i=0;i<moveableElements.Count;i++)
		{
			VisualElement visualElement = new VisualElement();
			visualElement.style.position = Position.Absolute;
			visualElement.style.backgroundColor = new Color(1, 1, 0, 0.5f);
			dynamicLayer.Add(visualElement);
			
			moveableElements[i].RegisterCallback<GeometryChangedEvent>((evt) => 
			{
				var element = evt.currentTarget as VisualElement;
				visualElement.style.width = element.worldBound.width;
				visualElement.style.height = element.worldBound.height;
				visualElement.transform.position = element.worldBound.position;
			});
			
			dynamicUIDictionary.Add(visualElement.GetHashCode(), moveableElements[i]);
			
			visualElement.RegisterCallback<PointerDownEvent>((evt) => 
			{
				Touch touch = TouchExtension.GetTouchOverlapVisualElement(visualElement, dynamicLayer.panel);
				StartCoroutine(MoveDynamicUI(touch, visualElement, dynamicUIDictionary[visualElement.GetHashCode()]));
			});
		}
	}
	
	IEnumerator MoveDynamicUI(Touch touch, VisualElement controlElement, VisualElement manipulatedElement)
	{
		manipulatedElement.style.right = manipulatedElement.style.left = manipulatedElement.style.top = manipulatedElement.style.bottom = 0;
		while (touch.phase != UnityEngine.InputSystem.TouchPhase.Ended)
		{
			Vector2 touchPosition = RuntimePanelUtils.ScreenToPanel(controlElement.panel, new Vector2(touch.screenPosition.x, Screen.height - touch.screenPosition.y));
			controlElement.transform.position = controlElement.parent.WorldToLocal(touchPosition);
			manipulatedElement.transform.position = manipulatedElement.parent.WorldToLocal(touchPosition);
		
			yield return new WaitForSeconds(Time.fixedDeltaTime);	
		}
	}
}