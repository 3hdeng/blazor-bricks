//js, window vs document ???
window.exampleJsFunctions = {
  showPrompt: function (text) {
    return prompt(text, 'Type your name here');
  },
  displayWelcome: function (welcomeMessage) {
    document.getElementById('welcome').innerText = welcomeMessage;
  },
  returnArrayAsyncJs: function () {
    DotNet.invokeMethodAsync('BlazorSample', 'ReturnArrayAsync')
      .then(data => {
        data.push(4);
          console.log(data);
      });
  },
  sayHello: function (dotnetHelper) {
    return dotnetHelper.invokeMethodAsync('SayHello')
      .then(r => console.log(r));
  }
};

/*const assemblyName = 'BlasmBricks';
const namespace = 'BlasmBricks';
const typeName = 'OnKeyUp';
const methodName = 'Handler';
*/
//const onkeyupMethod = Blazor.platform.findMethod(
//	assemblyName,
//	namespace,
//	typeName,
//	methodName
//	);

window.document.onkeyup = function (evt) {
  evt = evt || window.event;
  //const keyCode = Blazor.platform.toDotNetString(evt.keyCode.toString());
  const keyCode = evt.keyCode.toString();
  //Blazor.platform.callMethod(onkeyupMethod, null, [keyCode]);
DotNet.invokeMethodAsync('BlasmBricks', 'OnKeyUpHandler', keyCode)

};

/*function onKeyUp(element, evt) {
  const char = Blazor.platform.toDotNetString(evt.key)
	Blazor.platform.callMethod(onkeyupMethod, null, [char]);
}
*/

