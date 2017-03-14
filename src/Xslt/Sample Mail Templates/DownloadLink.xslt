<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:msxsl="urn:schemas-microsoft-com:xslt"
xmlns:user="urn:my-scripts"
xmlns:FormsExt="urn:FormsExt.Xslt.Library" 
xmlns:umbraco.library="urn:umbraco.library"
exclude-result-prefixes="xsl msxsl user umbraco.library FormsExt">

  <xsl:output method="html" media-type="text/html" doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN"
  doctype-system="DTD/xhtml1-strict.dtd"
  cdata-section-elements="script style"
  indent="yes"
  encoding="utf-8"/>

  <xsl:param name="records" />

  <xsl:template match="/">

    <xsl:variable name="email">
      <xsl:value-of select="$records//fields/*[caption='E-Mail']//value"/>
    </xsl:variable>
    <xsl:variable name="tracking">
      <xsl:value-of select="FormsExt:GenerateTrackingToken($email)"/>
    </xsl:variable>
    <xsl:variable name="downloadkey">
      <xsl:value-of select="$records//fields/*[caption='DownloadKey']//value"/>
    </xsl:variable>
    <xsl:variable name="downloadlink">http://localhost:56857/umbraco/FormsExt/Download/Media?key=<xsl:value-of select="$downloadkey"/>&amp;t=<xsl:value-of select="$tracking"/></xsl:variable>

    <body style="font-family: Segoe UI, Frutiger, Frutiger Linotype, Dejavu Sans, Helvetica Neue, Arial, sans-serif; font-size: 16px; line-height: 20px;">

      <table width="80%" style="max-width:80%;max-width:500px;font-family: Segoe UI, Frutiger, Frutiger Linotype, Dejavu Sans, Helvetica Neue, Arial, sans-serif;">
        <tr>
          <td align="center">

            <p style="font-size:120px;">
              <a href="{$downloadlink}" style="text-decoration:none;color:#e2e2e2;">🌠</a>
            </p>

            <h3>Hier ist Ihr Download!</h3>
            <p>
              Vielen Dank für Ihr Interesse. Wenn Sie Anregungen oder Fragen haben, antworten Sie infach auf diese E-Mail.
            </p>

            <table width="80%" style="max-width:80%;font-family: Segoe UI, Frutiger, Frutiger Linotype, Dejavu Sans, Helvetica Neue, Arial, sans-serif;">
              <tr>
                <td>
                  <table border="0" align="center" cellpadding="0" cellspacing="0" style="margin:0 auto;">
                    <tr>
                      <td align="center">
                        <table border="0" cellpadding="0" cellspacing="0" style="margin:0 auto;">
                          <tr>
                            <td align="center" bgcolor="#ffcc00" style="min-width:180px; -moz-border-radius: 3px; -webkit-border-radius: 3px; border-radius: 3px; font-family: Segoe UI, Frutiger, Frutiger Linotype, Dejavu Sans, Helvetica Neue, Arial, sans-serif;color:#ffffff;">
                              <a class="msoAltFont" href="{$downloadlink}" style="padding: 9px 12px; mso-padding-alt: 9px 12px; min-width:180px; display: block;text-decoration: none;border:0; text-align: center; text-transform:uppercase; font-weight: 600;font-size: 16px; color: #ffffff; background: #ffcc00; border: 1px solid #ffcc00;-moz-border-radius: 3px; -webkit-border-radius: 3px; border-radius: 3px; mso-line-height-rule: exactly; line-height:18px;">
                                DOWNLOAD
                              </a>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>
            </table>

            <p style="color:#333;font-size:10px;">
              Lorem ipsum dolor ...
            </p>


            <p style="color:#333;font-size:10px;">
              <br/><br/>
              <img src="http://mindrevolution.com/static/mindrevolution-square-signet.png" width="60" style="width:60px"/>
              <br/><br/>
              
              mindrevolution GmbH<br/>Theodor-Heuss-Straße 23<br/>70174 Stuttgart<br/>Germany
              <br/><br/>86 - 90 Paul Street<br/>London EC2A 4NE<br/>United Kingdom
              <br/><br/>
              <a href="http://www.mindrevolution.com">www.mindrevolution.com</a><br/>
              Phone +49 711 3401740 / +44 20 03331340<br/>
              <a href="mailto:stuttgart@mindrevolution.com">stuttgart@mindrevolution.com</a><br/>
              <a href="mailto:london@mindrevolution.com">london@mindrevolution.com</a><br/>
              <a href="https://twitter.com/mindrevolution">@mindrevolution</a>
              <br/><br/>
              <a href="http://www.mindrevolution.com/de/impressum">IMPRESSUM</a><br/>
            </p>



          </td>
        </tr>
      </table>
     
    </body>

  </xsl:template>



</xsl:stylesheet>