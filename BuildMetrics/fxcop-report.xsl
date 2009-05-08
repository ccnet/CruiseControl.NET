<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="html" />
	<xsl:param name="applicationPath" select="'.'" />

	<xsl:variable name="fxcop.root" select="//FxCopReport[@Version = '1.36']" />

	<xsl:template match="/">
		<div id="fxcop-report">
			<script language="javascript">
				function toggle (name, img)
				{
				var element = document.getElementById (name);

				if (element.style.display == 'none')
				element.style.display = '';
				else
				element.style.display = 'none';

				var img_element = document.getElementById (img);

				if (img_element.src.indexOf ('minus.png') > 0)
				img_element.src = '<xsl:value-of select="$applicationPath" />/images/plus.png';
				else
				img_element.src = '<xsl:value-of select="$applicationPath" />/images/minus.png';
				}
			</script>
			<style type="text/css">
				#fxcop-report
				{
				font-family: Arial, Helvetica, sans-serif;
				margin-left: 0;
				margin-right: 0;
				margin-top: 0;
				}

				#fxcop-report .header
				{
				background-color: #566077;
				background-repeat: repeat-x;
				color: #fff;
				font-weight: bolder;
				height: 50px;
				vertical-align: middle;
				}

				#fxcop-report .headertext
				{
				height: 35px;
				margin-left: 15px;
				padding-top: 15px;
				width: auto;
				}

				#fxcop-report .wrapper
				{
				padding-left: 20px;
				padding-right: 20px;
				width: auto;
				}

				#fxcop-report .legend
				{
				background-color: #ffc;
				border: #d7ce28 1px solid;
				font-size: small;
				margin-top: 15px;
				padding: 5px;
				vertical-align: middle;
				width: inherit;
				}

				#fxcop-report .clickablerow
				{
				cursor: pointer;
				}

				#fxcop-report .tabletotal
				{
				border-top: 1px #000;
				font-weight: 700;
				}

				#fxcop-report .results-table
				{
				border-collapse: collapse;
				font-size: 12px;
				margin-top: 20px;
				text-align: left;
				width: 100%;
				}

				#fxcop-report .results-table th
				{
				background: #b9c9fe;
				border-bottom: 1px solid #fff;
				border-top: 4px solid #aabcfe;
				color: #039;
				font-size: 13px;
				font-weight: 400;
				padding: 8px;
				}

				#fxcop-report .results-table td
				{
				background: #e8edff;
				border-bottom: 1px solid #fff;
				border-top: 1px solid transparent;
				color: #669;
				padding: 5px;
				}

				#fxcop-report .errorlist td
				{
				background: #FFF;
				border-bottom: 0;
				border-top: 0 solid transparent;
				color: #000;
				padding: 0;
				}

				#fxcop-report .inner-results
				{
				border-collapse: collapse;
				font-size: 12px;
				margin-bottom: 3px;
				margin-top: 4px;
				text-align: left;
				width: 100%;
				}

				#fxcop-report .inner-results td
				{
				background: #FFF;
				border-bottom: 1px solid #fff;
				border-top: 1px solid transparent;
				color: #669;
				padding: 3px;
				}

				#fxcop-report .inner-header th
				{
				background: #b9c9fe;
				color: #039;
				}

				#fxcop-report .inner-rule-description
				{
				background-color: transparent;
				border-collapse: collapse;
				border: 0px;
				font-size: 12px;
				margin-bottom: 3px;
				margin-top: 4px;
				text-align: left;
				width: 100%;
				}

				#fxcop-report .inner-rule-description tr
				{
				background-color: transparent;
				border: 0px;
				}

				#fxcop-report .inner-rule-description td
				{
				background-color: transparent;
				border: 0px;
				}
			</style>
			<xsl:apply-templates select="$fxcop.root" />
		</div>
	</xsl:template>

	<xsl:template match="FxCopReport">
		<div class="header">
			<div class="headertext">
				FxCop <xsl:value-of select="@Version"/>: <xsl:value-of select="Localized/String[@Key='ReportTitle']/text()" />
			</div>
		</div>
		<div class="wrapper">
			<div class="legend">
				<div>
					Assemblies checked: <xsl:value-of select="count(Targets/Target/Modules/Module)" /><br />
					Warnings: <xsl:value-of select="count(//Message/Issue[@Level='Warning'])" /><br />
					Critical Warnings: <xsl:value-of select="count(//Message/Issue[@Level='CriticalWarning'])" /><br />
					Errors: <xsl:value-of select="count(//Message/Issue[@Level='Error'])"/><br />
					Critical Errors: <xsl:value-of select="count(//Message/Issue[@Level='CriticalError'])"/><br />
					Total Messages: <xsl:value-of select="count(//Message)"/><br />
				</div>
			</div>
			<table class='results-table'>
				<thead>
					<tr>
						<th scope='col'></th>
						<th scope='col'></th>
						<th scope='col'>Assembly</th>
						<th scope='col'>Types</th>
						<th scope='col'>Warnings</th>
						<th scope='col'>Errors</th>
						<th scope='col'>Total Messages</th>
					</tr>
				</thead>
				<tbody>
					<xsl:for-each select="Targets/Target/Modules/Module">
						<xsl:call-template name="print-module" />
					</xsl:for-each>
				</tbody>
			</table>
		</div>
	</xsl:template>

	<xsl:template name="print-module">
		<xsl:variable name="module.id" select="generate-id()" />

		<tr class="clickablerow" onclick="toggle('{$module.id}', 'img-{$module.id}')">
			<td style="width: 10px">
				<img id="img-{$module.id}" src="{$applicationPath}/images/plus.png" />
			</td>
			<td style="width: 16px">
				<xsl:choose>
					<xsl:when test="count(.//Message/Issue[@Level='Error' or @Level='CriticalError']) > 0">
						<img src="{$applicationPath}/images/error.png" />
					</xsl:when>
					<xsl:when test="count(.//Message/Issue[@Level='Warning' or @Level='CriticalWarning']) > 0">
						<img src="{$applicationPath}/images/warning.png" />
					</xsl:when>
					<xsl:otherwise>
						<img src="{$applicationPath}/images/ok.png" />
					</xsl:otherwise>
				</xsl:choose>
			</td>
			<td>
				<xsl:value-of select="@Name" />
			</td>
			<td>
				<xsl:value-of select="count(.//Types/Type)" />
			</td>
			<td>
				<xsl:value-of select="count(.//Message/Issue[@Level='Warning' or @Level='CriticalWarning'])" />
			</td>
			<td>
				<xsl:value-of select="count(.//Message/Issue[@Level='Error' or @Level='CriticalError'])"/>
			</td>
			<td>
				<xsl:value-of select="count(.//Message)"/>
			</td>
		</tr>
		<xsl:call-template name="print-module-error-list">
			<xsl:with-param name="module.id" select="$module.id"/>
		</xsl:call-template>
	</xsl:template>

	<xsl:template name="print-module-error-list">
		<xsl:param name="module.id" />

		<tr id="{$module.id}" class="errorlist" style="display: none">
			<td></td>
			<td colspan="6">
				<table cellpadding="2" cellspacing="0" width="100%" class="inner-results">
					<thead>
						<tr class="inner-header">
							<th scope='col'></th>
							<th scope='col'></th>
							<th scope='col'>Location</th>
							<th scope='col'>Fix Category</th>
							<th scope='col'>Certainty</th>
							<th scope='col'>Rule</th>
						</tr>
					</thead>
					<tbody>
						<xsl:for-each select=".//Messages/Message">
							<xsl:variable name="message.id" select="generate-id()" />
							<xsl:variable name="rule.check.id" select="@CheckId" />
							<xsl:variable name="rule.name" select="$fxcop.root/Rules/Rule[@CheckId = $rule.check.id]/Name" />
							<xsl:variable name="rule.category" select="$fxcop.root/Rules/Rule[@CheckId = $rule.check.id]/@Category" />
							<xsl:variable name="rule.description" select="$fxcop.root/Rules/Rule[@CheckId = $rule.check.id]/Description" />
							<xsl:variable name="rule.url" select="$fxcop.root/Rules/Rule[@CheckId = $rule.check.id]/Url" />
							<xsl:variable name="rule.file.name" select="$fxcop.root/Rules/Rule[@CheckId = $rule.check.id]/File/@Name" />
							<xsl:variable name="rule.file.version" select="$fxcop.root/Rules/Rule[@CheckId = $rule.check.id]/File/@Version" />

							<tr class="clickablerow" onclick="toggle('{$module.id}-{$message.id}', 'img-{$module.id}-{$message.id}')">
								<td style="width: 10px">
									<img id="img-{$module.id}-{$message.id}" src="{$applicationPath}/images/plus.png" />
								</td>
								<td style="width: 16px">
									<xsl:choose>
										<xsl:when test="./Issue/@Level = 'Warning'">
											<img src="{$applicationPath}/images/warning.png">
												<xsl:attribute name="title">
													<xsl:value-of select="./Issue/@Level" />
												</xsl:attribute>
											</img>
										</xsl:when>
										<xsl:when test="./Issue/@Level = 'Error' or 'CriticalError'">
											<img src="{$applicationPath}/images/error.png">
												<xsl:attribute name="title">
													<xsl:value-of select="./Issue/@Level" />
												</xsl:attribute>
											</img>
										</xsl:when>
										<xsl:otherwise>
											<img src="{$applicationPath}/images/ok.png">
												<xsl:attribute name="title">
													<xsl:value-of select="./Issue/@Level" />
												</xsl:attribute>
											</img>
										</xsl:otherwise>
									</xsl:choose>
								</td>
								<td>
									<xsl:value-of select="../../../../@Name" /> -	<xsl:value-of select="../../@Name" />
								</td>
								<td>
									<xsl:value-of select="@FixCategory" />
								</td>
								<td style="text-align: center">
									<xsl:value-of select="./Issue/@Certainty" /> %
								</td>
								<td>
									<xsl:value-of select="$rule.name" />
								</td>
							</tr>
							<tr id="{$module.id}-{$message.id}" style="display: none">
								<td></td>
								<td colspan="5">
									<div class="legend">
										<div>
											<table cellpadding="2" cellspacing="0" width="100%" class="inner-rule-description">
												<tr>
													<td>
														<b>Rule:</b>
													</td>
													<td>
														<xsl:value-of select="$rule.name" />
													</td>
												</tr>
												<tr>
													<td>
														<b>Check Id:</b>
													</td>
													<td>
														<xsl:value-of select="$rule.check.id" />
													</td>
												</tr>
												<tr>
													<td>
														<b>Category:</b>
													</td>
													<td>
														<xsl:value-of select="$rule.category" />
													</td>
												</tr>
												<tr>
													<td>
														<b>Description:</b>
													</td>
													<td>
														<xsl:value-of select="$rule.description" />
													</td>
												</tr>
												<tr>
													<td>
														<b>Found at:</b>
													</td>
													<td>
														<xsl:if test="./Issue/@Path">
															<xsl:value-of select="./Issue/@Path" /> at line <xsl:value-of select="./Issue/@Line" />
														</xsl:if>
													</td>
												</tr>
												<tr>
													<td>
														<b>Resolution:</b>
													</td>
													<td>
														<xsl:value-of select="./Issue" />
													</td>
												</tr>
												<tr>
													<td>
														<b>Help Link:</b>
													</td>
													<td>
														<a href="{$rule.url}" target="_blank">
															<xsl:value-of select="$rule.url" />
														</a>
													</td>
												</tr>
												<tr>
													<td>
														<b>Rule File:</b>
													</td>
													<td>
														<xsl:value-of select="$rule.file.name" /> Version: <xsl:value-of select="$rule.file.version" />
													</td>
												</tr>
											</table>
										</div>
									</div>
								</td>
							</tr>
						</xsl:for-each>
					</tbody>
				</table>
			</td>
		</tr>
	</xsl:template>
</xsl:stylesheet>