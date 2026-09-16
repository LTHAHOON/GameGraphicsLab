# 게임 그래픽스 포트폴리오

## 구현 설명

* 좌클릭을 누를 때 다트가 날라가고 돌림판이 돌아갑니다.

* 다트 돌림판이 끝나면 폭죽이 나오도록 설계하였습니다.

* SG_ToonBand은 CustomFunction을 통해 메인 라이트 정보를 가져와서 햇빛이 반영되게 하였습니다.(Direction Light를 방향이 바꾸면 Toon이 변하게 됩니다.)

* SG_Water_01은 프레넬과 Scene Depth을 통해 가까우면 색이 연하며 왜곡이 약해지고 멀리있으면 색이 진해지며 왜곡이 강해지게 만들었습니다.

* 또한 SG_Water_01은 Reflection Probe 컴포넌트를 통해 Reflection Cubemap을 베이크해서 반사가 적용될 수있도록 만들었고 Depth Fade 노드로 물과 물 위에 있는 오브젝트와의 깊이 차이를 계산하여 경계선 거품효과도 만들었습니다.

* 물 속 안에 들어가면 화면 색이 바뀌도록 Water Volume을 설정해놓았습니다.

* Pt_Water_Sphere가 물에 닿으면 Trigger로 물보라(Spray)가 생성되도록 풀링으로 만들었습니다.

* SG_Film같은 경우 FullScreen 렌더 피처로 사용할 수 있게 만들었고 렌더 타이밍을 Post Processing 이후로 잡아서 필름이 뭉개지지않도록 만들었습니다. (일단 꺼두었습니다.)

* Global Volume은 현실적인 느낌을 주기 위해 대표적으로 Depth Of Field를 사용하여 시력 효과를 주었습니다.