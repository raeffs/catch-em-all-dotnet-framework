using System.Collections.Generic;
using Cirrus.Engine.ViewModel;
using Cirrus.Engine.Workflow;
using Cirrus.Engine.Workflow.Attributes;
using Cirrus.Module.Core.Steps.UserInput;

namespace Cirrus.Module.CatchEmAll.Steps.Interactive
{
    /// <summary>
    /// Basic interface for selecting elements.
    /// </summary>
    public interface ISelectStep : IWorkflowStep, IHasDashboardComponentConfiguration, IHasUniqueWorkflowStepId
    {
        /// <summary>
        /// Gets the "back" edge, returned when the user request the previous step.
        /// </summary>
        /// <value>
        /// The "back" edge.
        /// </value>
        EdgeDefinition Back { get; }

        /// <summary>
        /// Gets the "canceled" edge, returned when the user cancled the request.
        /// </summary>
        /// <value>
        /// The "canceled".
        /// </value>
        EdgeDefinition Canceled { get; }

        /// <summary>
        /// Gets the "create" edge, returned when the user request to create a item.
        /// </summary>
        /// <value>
        /// The "create" edge.
        /// </value>
        EdgeDefinition Create { get; }

        /// <summary>
        /// Gets the "delete" edge, returned when the user request to delete multiple items.
        /// </summary>
        /// <value>
        /// The "delete" edge.
        /// </value>
        EdgeDefinition Delete { get; }

        /// <summary>
        /// Gets the "edit" edge, returned when the user request to edit a item.
        /// </summary>
        /// <value>
        /// The "edit" edge.
        /// </value>
        EdgeDefinition Edit { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the [back edge can be called].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [back edge can be called]; otherwise, <c>false</c>.
        /// </value>
        [Input(Optional = true)]
        bool CanGoBack { get; set; }

        /// <summary>
        /// Gets or sets the help identifier.
        /// </summary>
        /// <value>
        /// The help identifier.
        /// </value>
        [Input(Optional = true)]
        string HelpId { get; set; }

        /// <summary>
        /// Gets or sets the multi select identifiers, if a multi select was performed..
        /// </summary>
        /// <value>
        /// The multi select identifiers.
        /// </value>
        [Output]
        ICollection<long> MultiSelectIds { get; set; }

        /// <summary>
        /// Gets or sets the single select identifier, if a single select was performed.
        /// </summary>
        /// <value>
        /// The single select identifier.
        /// </value>
        [Output]
        long SingleSelectId { get; set; }

        /// <summary>
        /// Gets or sets the filter for sort ordering.
        /// </summary>
        [Output(ResumeOldValues = true)]
        Filter Filter { get; set; }
    }
}