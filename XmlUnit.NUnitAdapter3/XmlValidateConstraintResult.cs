namespace NetBike.XmlUnit.NUnitAdapter
{
    using global::NUnit.Framework.Constraints;
    using NetBike.XmlUnit;

    internal class XmlValidateConstraintResult : ConstraintResult
    {
        private readonly XmlValidationResult validationResult;

        public XmlValidateConstraintResult(IConstraint constraint, object actualValue, XmlValidationResult validationResult)
            : base(constraint, actualValue)
        {
            this.validationResult = validationResult;
            this.Status = validationResult.IsSuccess ? ConstraintStatus.Success : ConstraintStatus.Failure;
        }

        public override void WriteMessageTo(MessageWriter writer)
        {
            writer.Write(this.validationResult.ToString());
        }
    }
}
