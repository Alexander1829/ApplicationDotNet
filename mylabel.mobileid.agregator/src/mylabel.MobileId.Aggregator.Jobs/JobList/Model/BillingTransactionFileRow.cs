using System.ComponentModel.DataAnnotations.Schema;
using mylabel.MobileId.Aggregator.Db;

namespace mylabel.MobileId.Aggregator.Jobs.JobList.Model
{
	public class BillingTransactionFileRow 
	{
		public BillingTransactionFileRow(BillingTransaction t)
		{
			at_feature_code = t.at_feature_code!;
			basic_service_code = t.basic_service_code!;
			basic_service_type = t.basic_service_type!;
			Calling_no_ggsn = t.Calling_no_ggsn!;
			call_action_code = t.call_action_code!;
			call_destination = t.call_destination!;
			call_source = t.call_source!;
			call_to_tn_sgsn = t.call_to_tn_sgsn!;
			channel_seizure_date_time = t.channel_seizure_date_time!.Value.ToString("yyyyMMddHHmmss");
			guide_by = t.guide_by!;
			message_switch_id = t.message_switch_id!;
			record_type = t.record_type!;
			subscriber_no = t.subscriber_no!;
			technology = t.technology!;
			uom = t.uom!;
		}

		//Зачем-то Биллингу, в файле, нужно огромное количество пустых полей, причём, в фиксированном порядке. В этом инстансе сверху вниз:
		public string record_type { get; set; }
		public string subscriber_no { get; set; }
		public string channel_seizure_date_time { get; set; }
		public string message_switch_id { get; set; }
		public long us_seq_no { get; set; }
		public string at_feature_code { get; set; }
		public string call_action_code { get; set; }
		public string? feature_selection_dt { get; set; }
		public long at_call_dur_sec { get; set; }
		public string call_to_tn_sgsn { get; set; }
		public string Calling_no_ggsn { get; set; }
		public string? mps_file_number { get; set; }
		public string? message_type { get; set; }
		public string? duration { get; set; }
		public string guide_by { get; set; }
		public string? outcollect_ind { get; set; }
		public string? catchup_ind { get; set; }
		public int data_volume { get; set; }
		public string? original_amt { get; set; }
		public string? original_amt_gn { get; set; }
		public string? imsi { get; set; }
		public string? imei { get; set; }
		public string? event_type { get; set; }
		public string? cell_id { get; set; }
		public long ac_amt { get; set; }
		public string? call_forward_ind { get; set; }
		public string? lac { get; set; }
		public string? provider_id { get; set; }
		public string call_source { get; set; }
		public string? rm_tax_amt_air { get; set; }
		public string? waived_call_ind { get; set; }
		public string basic_service_code { get; set; }
		public string basic_service_type { get; set; }
		public string? dialed_digits { get; set; }
		public string? record_id { get; set; }
		public string? tax_id { get; set; }
		public string? calculate_uc_rate_ind { get; set; }
		public string? sdr_amount { get; set; }
		public string uom { get; set; }
		public string? supplementry_srvc_code { get; set; }
		public string? home_ctn { get; set; }
		public string technology { get; set; }
		public string? chanel_type { get; set; }
		public string? transparency_ind { get; set; }
		public string? ms_classmark { get; set; }
		public string? ss_param_ip_address { get; set; }
		public string? original_call_type { get; set; }
		public string? original_call_npi { get; set; }
		public string? original_call_number { get; set; }
		public string? sdr_exchange_rate { get; set; }
		public string? dual_service_type { get; set; }
		public string? dual_service_code { get; set; }
		public string? camel_served_address { get; set; }
		public string? camel_service_key { get; set; }
		public string? camel_msc_address { get; set; }
		public string? camel_ref_number { get; set; }
		public string? camel_dest { get; set; }
		public string? msc_chrg_type_ind { get; set; }
		public string? file_name { get; set; }
		public string? country_of_orig { get; set; }
		public string? rec_status { get; set; }
		public string? called_country_code { get; set; }
		public string? camel_charge { get; set; }
		public string? sdr_camel_charge { get; set; }
		public string? from_provider_id { get; set; }
		public string call_destination { get; set; }
		public string? ss_action_code { get; set; }
		public string? new_balance { get; set; }
		public string? Charging_id { get; set; }
		public string? dynamic_data { get; set; }
		[NotMapped]
		public string? SiOrDi { get; set; }
		[NotMapped]
		public string? AccessTokenOnAggregatorHash { get; set; }
	}
}
