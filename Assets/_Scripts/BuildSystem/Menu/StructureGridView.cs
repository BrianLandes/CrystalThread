﻿using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using frame8.ScrollRectItemsAdapter.Classic;
using UnityEngine;

public class StructureGridView : ClassicSRIA<CellViewsHolder> {

	public RectTransform itemPrefab;

	public List<StructureData> availableStructures;

	protected override void Awake() {
		base.Awake();

		//LoadStructureData();
		//ResetItems(Data.Count);
	}

	//private void LoadStructureData() {
	//	availableStructures = new List<StructureData>();
		
	//	Object[] datas = Resources.LoadAll("StructureData");
	//	foreach (var data in datas) {
	//		StructureData castData = data as StructureData;
	//		availableStructures.Add(castData);
	//	}
	//}


	protected override void Start() {
		base.Start();
		ResetItems(availableStructures.Count);
	}

	protected override CellViewsHolder CreateViewsHolder(int itemIndex) {
		var instance = new CellViewsHolder();
		instance.Init(itemPrefab, itemIndex);

		return instance;
	}

	protected override void UpdateViewsHolder(CellViewsHolder vh) {
		var model = availableStructures[vh.ItemIndex];
		vh.titleText.text = model.title;
		vh.image.sprite = model.icon;
		vh.button.onClick.RemoveAllListeners();
		vh.button.onClick.AddListener( () => OnStructureButtonClicked(vh.ItemIndex) );
	}

	private void OnStructureButtonClicked(int index) {
		//Debug.Log(index);
		var model = availableStructures[index];
		QueryManager.GetPlayer().GetComponent<BuildMenuController>().StartSettingBuilding(model);
	}
}

public class CellViewsHolder : CAbstractViewsHolder {
	public Text titleText;
	public Image image;
	public Button button;

	public override void CollectViews() {
		base.CollectViews();

		titleText = root.Find("TitleText").GetComponent<Text>();
		image = root.Find("ImagePanel/Image").GetComponent<Image>();
		button = root.GetComponent<Button>();
	}
}