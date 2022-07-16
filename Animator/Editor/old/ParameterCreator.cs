using UnityEngine;

namespace PC
{
	public static class ParameterCreator {

		public static AnimatorControllerParameter CreateParameter(string name,AnimatorControllerParameterType type)
        {
            //Create Normalized Parameter
            var param = new AnimatorControllerParameter();
            param.name = name;
            param.type = type;
            return param;
        }
	}
}
